using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;
using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Services.BemChat;
using CRMIntegration.Services.BemChat.DTOs.Requests;
using CRMIntegration.Services.CobMais;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CRMIntegration.Infra.Consumers.Consumers
{
    public class TriggerCampaignConsumer(
        IBemChatService bemChatService,
        ICobMaisService cobMaisService,
        IClientRepository clientRepository,
        ICampaignRepository campaignRepository,
        IUnitOfwork unitOfWork,
        ILogger<TriggerCampaignConsumer> logger
    ) : IConsumer<SendContactMessageCommand>
    {
        public async Task Consume(ConsumeContext<SendContactMessageCommand> context)
        {
            var cmd = context.Message;

            if (cmd is null)
            {
                logger.LogWarning("Mensagem nula recebida, ignorando.");
                return;
            }

            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = cmd.CorrelationId,
                ["CampaignId"] = cmd.CampaignId,
                ["PhoneNumber"] = cmd.PhoneNumber
            });

            var alreadySent = await campaignRepository.ExistsForClientInCampaignAsync(
                cmd.CampaignId, cmd.IdPessoa, context.CancellationToken);

            if (alreadySent)
            {
                logger.LogWarning(
                    "Mensagem para o cliente {IdPessoa} na campanha {CampaignId} já foi enviada. Ignorando.",
                    cmd.IdPessoa, cmd.CampaignId);
                return;
            }

            var client = await clientRepository.GetByIdCobMaisAsync(
                cmd.IdPessoa, context.CancellationToken);

            if (client is null)
            {
                client = new Client(
                    cmd.IdPessoa, cmd.Nome, cmd.PhoneNumber, cmd.CpfCnpj,
                    idTelefoneCobMais: cmd.IdPessoa);

                client.SynchronizeWithBemChat($"bemchat_{cmd.IdPessoa}");

                await clientRepository.AddAsync(client);

                logger.LogDebug(
                    "[BemChat] Novo cliente criado localmente: IdPessoa={IdPessoa} Nome={Nome}.",
                    cmd.IdPessoa, cmd.Nome);
            }
            else if (!client.IsSynchronizedWithBemChat())
            {
                client.SynchronizeWithBemChat($"bemchat_{cmd.IdPessoa}");
            }

            var campaign = await campaignRepository.GetByIdAsync(
                cmd.CampaignId, context.CancellationToken);

            if (campaign is null)
            {
                logger.LogError(
                    "Campanha {CampaignId} não encontrada. Abortando envio para {PhoneNumber}.",
                    cmd.CampaignId, cmd.PhoneNumber);
                return;
            }

            var message = new CampaignMessage(cmd.CampaignId, client.Id);
            await campaignRepository.AddMessageAsync(message, context.CancellationToken);

            var sendResponse = await bemChatService.SendTextMessageAsync(
                new SendTextMessageRequest(
                    Number: cmd.PhoneNumber,
                    Body: cmd.TemplateName
                ),
                context.CancellationToken);

            message.MarkAsDelivered(sendResponse.MessageId ?? $"bemchat_{Guid.NewGuid()}");
            client.MarkAsTriggered();

            await campaignRepository.IncrementSentCountAsync(
                campaign.Id, context.CancellationToken);

            if (client.IdTelefoneCobMais.HasValue)
            {
                await cobMaisService.MarkPhoneAsNonActionableAsync(
                    client.IdTelefoneCobMais.Value, context.CancellationToken);
                client.MarkAsNonActionable();
            }

            await unitOfWork.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation(
                "[BemChat] ✓ Mensagem enviada para {PhoneNumber} (IdPessoa={IdPessoa}).",
                cmd.PhoneNumber, cmd.IdPessoa);
        }
    }
}
