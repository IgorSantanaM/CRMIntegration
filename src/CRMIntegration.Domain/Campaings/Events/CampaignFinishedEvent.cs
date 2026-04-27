using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record CampaignFinishedEvent(Guid Id,
                                    DateTime DataFinalizacao,
                                    int TotalContatos,
                                    int TotalEnviados,
                                    int TotalEntregues,
                                    int TotalLidos,
                                    int TotalFalhas,
                                    decimal TaxaEntrega,
                                    decimal TaxaLeitura,
                                    decimal TaxaFalha) : Event<Guid>(Id);
}
