using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record MessageFailedEvent(Guid Id,
        Guid CampaignId,
        Guid ClientId,
        string ErrorMessage,
        DateTime FailedDate) : Event<Guid>(Id);
}
