using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record MessageDeliveredEvent(Guid Id,
        Guid CampaignId,
        Guid ClientId,
        string IdMensagemVoll,
        DateTime DeliveredDate) : Event<Guid>(Id);
}
