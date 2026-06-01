using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Domain.Core.Exceptions;
using CRMIntegration.Infra.Services.BemChat;
using CRMIntegration.Services.CobMais;
using MassTransit;
using MediatR;

namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign
{
    public class TriggerCampaignCommandHandler(
        IPublishEndpoint publishEndpoint,
        ICobMaisService cobMaisService,
        ICampaignRepository campaignRepository,
        IUnitOfwork unitOfWork,
        BemChatOptions bemChatOptions) : IRequestHandler<TriggerCampaignCommand>
    {
        public async Task Handle(TriggerCampaignCommand request, CancellationToken cancellationToken)
        {
            var exists = await campaignRepository.ExistsWithNameTodayAsync(
                request.TemplateName, cancellationToken);

            if (exists)
                throw new DomainException("Campaign already triggered today for this template.");

            var actionableContacts = await cobMaisService.GetActionableContactsAsync(
                new(request.StartDate, request.EndDate), cancellationToken);

            var filtered = actionableContacts.ToList();

            var campaign = new Campaign(
                nome: $"Campaign_{request.TemplateName}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                template: request.TemplateName,
                channelIdBemChat: bemChatOptions.DefaultWhatsAppId?.ToString() ?? "default",
                dataDisparo: DateTime.UtcNow,
                totalContatos: filtered.Count
            );

            campaign.StartProcessing();

            await campaignRepository.AddAsync(campaign);

            foreach (var contact in filtered)
            {
                await publishEndpoint.Publish<SendContactMessageCommand>(new(
                    campaign.Id,
                    Guid.NewGuid(),
                    contact.IdPessoa,
                    contact.CpfCnpj,
                    contact.Nome,
                    contact.PhoneNumber,
                    contact.PhoneId,
                    request.TemplateName
                ), cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
