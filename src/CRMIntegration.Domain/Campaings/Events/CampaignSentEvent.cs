using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record CampaignSentEvent(Guid Id,
        string IdCampanhaVoll,
        int TotalContatos,
        DateTime SentTime) : Event<Guid>(Id);
}
