using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record CampaignCancelledEvent(Guid Id, string Reason, DateTime CancelledTime) : Event<Guid>(Id);
}
