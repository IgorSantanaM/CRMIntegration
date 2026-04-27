using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events;

public record CampaignProcessingStartedEvent(Guid Id, DateTime? StartedTime) : Event<Guid>(Id);
