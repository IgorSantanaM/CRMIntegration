using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record MessageCreatedEvent(Guid Id, Guid CampaignId, Guid ClientId, DateTime SentTime) : Event<Guid>(Id);
}
