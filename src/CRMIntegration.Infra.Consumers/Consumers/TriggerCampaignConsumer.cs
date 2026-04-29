using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Domain.Core.Exceptions;
using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.Voll;
using CRMIntegration.Services.Voll.DTOs.Requests;
using CRMIntegration.Services.CobMais.DTOs.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;

namespace CRMIntegration.Infra.Consumers.Consumers
{
    public class TriggerCampaignConsumer : IConsumer<TriggerCampaignCommand>
    {
        private readonly IVollService _vollService;
        private readonly ICobMaisService _cobMaisService;
        private readonly IUnitOfwork _unitOfWork;
        private readonly IClientRepository _clientRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ILogger<TriggerCampaignConsumer> _logger;

        public TriggerCampaignConsumer(
            IVollService vollService,
            ICobMaisService cobMaisService,
            IUnitOfwork unitOfWork,
            IClientRepository clientRepository,
            ICampaignRepository campaignRepository,
            ILogger<TriggerCampaignConsumer> logger)
        {
            _vollService = vollService;
            _cobMaisService = cobMaisService;
            _unitOfWork = unitOfWork;
            _clientRepository = clientRepository;
            _campaignRepository = campaignRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TriggerCampaignCommand> context)
        {
            var command = context.Message;

            _logger.LogInformation("Processing TriggerCampaignCommand ID: {MessageId} for Template: {TemplateName}", context.MessageId, command.TemplateName);

            var cobMaisPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, ctx) =>
                    {
                        _logger.LogWarning(exception, "CobMais integration failed. Retry {RetryCount} after {Delay}s", retryCount, timeSpan.TotalSeconds);
                    });

            var contacts = await cobMaisPolicy.ExecuteAsync(() =>
                _cobMaisService.GetActionableContactsAsync(new GetActionableContactsRequest(command.StartDate, command.EndDate, null, null), context.CancellationToken));

            if (!contacts.Any())
            {
                _logger.LogInformation("No actionable contacts found for the given dates.");
                return;
            }

            var targetContacts = contacts.Where(c => c.Codigo == "802" || c.Codigo == null).ToList();

            if (!targetContacts.Any())
            {
                _logger.LogInformation("No contacts passing the business rule (e.g. Type=802).");
                return;
            }

            var campaign = new Campaign(
                nome: $"Campaign_{command.TemplateName}_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}",
                template: command.TemplateName,
                channelIdVoll: "default_channel", // TODO: CHANNEL ID.
                dataDisparo: DateTime.UtcNow,
                totalContatos: targetContacts.Count
            );

            var eventTasks = targetContacts.Select(c =>
            _cobMaisService.InsertCampaignDispatchEventAsync(
                c.IdPessoa.ToString(), command.TemplateName, context.CancellationToken)
            );

            await Task.WhenAll(eventTasks);

            var rateLimitPolicy = Policy.RateLimitAsync(80, TimeSpan.FromSeconds(1));

            var sendPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt),
                    onRetry: (exception, timeSpan, retryCount, ctx) =>
                    {
                        _logger.LogWarning(exception, $"Voll integration failed. Retry {retryCount} after {timeSpan.TotalSeconds}s");
                    });

            campaign.StartProcessing();

            var sendTasks = targetContacts.Select(async contact =>
            {
                try
                {
                    await rateLimitPolicy.ExecuteAsync(async () =>
                    {
                        await sendPolicy.ExecuteAsync(async () =>
                        {
                            var sendMessageRequest = new SendTemplateMessageRequest
                            (
                                contact.PhoneNumber,
                                new Services.Voll.DTOs.CampaignTemplateDto
                                (
                                    new Services.Voll.DTOs.CampaignTemplateLanguage("deterministic", "pt_BR"),
                                    command.TemplateName,
                                    new List<Services.Voll.DTOs.CampaignTemplateComponentDto>()
                                ),
                                "individual",
                                "template",
                                "0"
                            );

                            var response = await _vollService.SendTemplateMessageAsync(sendMessageRequest, context.CancellationToken);

                            var client = await _clientRepository.GetByIdCobMaisAsync(contact.IdPessoa, context.CancellationToken);
                            if (client is not null)
                            {
                                var message = new CampaignMessage(campaign.Id, client.Id);
                                campaign.AddMessage(message);

                                campaign.IncrementSent();
                                campaign.IncrementDelivered();
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send message to contact {Phone}", contact.PhoneNumber);
                    campaign.IncrementFail();
                }
            });

            await Task.WhenAll(sendTasks);

            await _campaignRepository.UpdateRangeAsync(campaign.Mensagens);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Finished processing TriggerCampaignCommand ID: {MessageId}. Success: {Sent}, Failed: {Failed}", 
                context.MessageId, campaign.TotalEnviados, campaign.TotalFalhas);
        }
    }
}
