using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;
using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Clients.Events;
using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Infra.Services.Voll;
using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.Voll;
using CRMIntegration.Services.Voll.DTOs;
using CRMIntegration.Services.Voll.DTOs.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CRMIntegration.Infra.Consumers.Consumers
{
    public class TriggerCampaignConsumer(IVollService vollService,
        ICobMaisService cobMaisService,
        IClientRepository clientRepository,
        ICampaignRepository campaignRepository,
        IUnitOfwork unitOfWork,
        VollOptions vollOptions,
        ILogger<TriggerCampaignConsumer> logger
        ) : IConsumer<SendContactMessageCommand>
    {
        public async Task Consume(ConsumeContext<SendContactMessageCommand> context)
        {
            SendContactMessageCommand? cmd = context.Message;

            if (cmd is null)
            {
                logger.LogInformation("Received null message, ignoring.");
                return;
            }

            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = cmd.CorrelationId,
                ["CampaignId"] = cmd.CampaignId,
                ["PhoneNumber"] = cmd.PhoneNumber
            });

            var alreadySent = await campaignRepository.ExistsForClientInCampaignAsync(cmd.CampaignId,
                cmd.IdPessoa,
                context.CancellationToken);

            if (alreadySent)
            {
                logger.LogWarning($"Message for client {cmd.IdPessoa} in campaign {cmd.CampaignId} " +
                    $"has already been sent, skipping.");
                return;
            }

            var client = await clientRepository.GetByIdCobMaisAsync(
           cmd.IdPessoa, context.CancellationToken);

            if (client is null)
            {
                client = new Client(
                    cmd.IdPessoa, cmd.Nome, cmd.PhoneNumber, cmd.CpfCnpj,
                    idTelefoneCobMais: cmd.IdPessoa);

                var request = new CreateContactRequest(
                    Whatsapp: cmd.PhoneNumber,
                    ChatName: cmd.Nome,
                    Email: null,
                    ChatOperator: null,
                    CustomFields:
                    [
                        new("cf_cobmais_id", cmd.IdPessoa.ToString()),
                        new("cf_cpfcnpj", cmd.CpfCnpj)
                    ]
                );

                var vollContactResponse = await vollService.CreateContactAsync(request, context.CancellationToken);

                client.SynchronizeWithVoll(vollContactResponse.Id);
                await clientRepository.AddAsync(client);
            }
            else if (!client.IsSynchronizedWithVoll())
            {
                CreateContactRequest request = new(cmd.PhoneNumber, cmd.Nome, null, null, [
                        new("cf_cobmais_id", cmd.IdPessoa.ToString()),
                        new("cf_cpfcnpj", cmd.CpfCnpj)
                    ]);

                var vollId = await vollService.GetOrCreateContactAsync(
                    request,
                    context.CancellationToken);
                client.SynchronizeWithVoll(vollId);
            }

            var campaign = await campaignRepository.GetByIdAsync(cmd.CampaignId, context.CancellationToken);

            if(campaign is null)
            {
                logger.LogError($"Campaign with id {cmd.CampaignId} not found, aborting."); 
            }

            var message = new CampaignMessage(
                cmd.CampaignId,
                client.Id);
            await campaignRepository.AddMessageAsync(message, context.CancellationToken);
            await unitOfWork.SaveChangesAsync(context.CancellationToken);

            var sendResponse = await vollService.SendTemplateMessageAsync(
            new SendTemplateMessageRequest(
                cmd.PhoneNumber,
                new CampaignTemplateDto(
                    new CampaignTemplateLanguage("deterministic", "pt_BR"),
                    cmd.TemplateName,
                    []
                ),
                "individual",
                "template",
                null
            ), context.CancellationToken);

            message.MarkAsDelivered(sendResponse.MessageId);

            client.MarkAsTriggered();
            campaign!.IncrementSent();

            if(client.IdTelefoneCobMais.HasValue)
            {
                await cobMaisService.MarkPhoneAsActionableAsync(client.IdTelefoneCobMais.Value, context.CancellationToken);
                client.MarkAsNonActionable();
            }

            await unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }
}
