using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Domain.Core.Exceptions;
using CRMIntegration.Infra.Services.Voll;
using CRMIntegration.Services.CobMais;
using MassTransit;
using MediatR;
using System.Threading.Channels;

namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign
{
    public class TriggerCampaignCommandHandler(IPublishEndpoint publishEndpoint,
        ICobMaisService cobMaisService,
        ICampaignRepository campaignRepository,
        IUnitOfwork unitOfWork,
        VollOptions vollOptions) : IRequestHandler<TriggerCampaignCommand>
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
                channelIdVoll: vollOptions.ChannelId,
                dataDisparo: DateTime.UtcNow,
                totalContatos: filtered.Count
            );

            campaign.StartProcessing();

            await campaignRepository.AddAsync(campaign);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var publishChannel = Channel.CreateBounded<SendContactMessageCommand>(new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = false,
                SingleReader = false
            });

            var producer = Task.Run(async () =>
            {
                foreach (var contact in filtered)
                {
                    await publishChannel.Writer.WriteAsync(new SendContactMessageCommand
                    (
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
                publishChannel.Writer.Complete();
            }, cancellationToken);

            var semaphore = new SemaphoreSlim(80);
            var consumer = Task.Run(async () =>
            {
                var publishTasks = new List<Task>();
                await foreach (var msg in publishChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    await semaphore.WaitAsync(cancellationToken);
                    publishTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await publishEndpoint.Publish(msg, cancellationToken);
                        }
                        finally
                        {
                            _ = Task.Delay(TimeSpan.FromSeconds(1), cancellationToken)
                                  .ContinueWith(_ => semaphore.Release(), cancellationToken);
                        }
                    }, cancellationToken));

                    publishTasks.RemoveAll(t => t.IsCompleted);
                }
                await Task.WhenAll(publishTasks);
            }, cancellationToken);

            await Task.WhenAll(producer, consumer);
        }
    }
}
