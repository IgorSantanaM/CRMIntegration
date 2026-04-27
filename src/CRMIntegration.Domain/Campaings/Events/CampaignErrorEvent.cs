using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record CampaignErrorEvent(Guid Id,
        string ErrorMessage,
        DateTime ErrorTime) : Event<Guid>(Id);
}
