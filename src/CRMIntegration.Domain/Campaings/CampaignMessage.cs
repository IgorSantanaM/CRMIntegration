using CRMIntegration.Domain.Campaings.Enum;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Core.Model;

namespace CRMIntegration.Domain.Campaings
{
    public class CampaignMessage : Entity<Guid>
    {
        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; }
        public string IdMensagemVoll { get; set; } = string.Empty;
        public CampaignStatus Status { get; set; } = CampaignStatus.Enviada;
        public DateTime DataEnvio { get; set; }
        public DateTime? DataEntrega { get; set; }
        public DateTime? DataLeitura { get; set; }
        public DateTime? DataFalha { get; set; }
        public string? WebhookPayload { get; set; }
        public string? MensagemErro { get; set; }
    }
}
