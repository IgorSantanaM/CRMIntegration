using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record CampaignMetricsUpdatedEvent(Guid Id,
                int TotalEnviados,
                int TotalEntregues,
                int TotalLidos,
                int TotalFalhas,
                DateTime DataAtualizacao) : Event<Guid>(Id);
}
