using CRMIntegration.Domain.Campaings.Enum;
using CRMIntegration.Domain.Campaings.Events;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Core.Exceptions;
using CRMIntegration.Domain.Core.Model;

namespace CRMIntegration.Domain.Campaings
{
    public class CampaignMessage : Entity<Guid>
    {
        public Guid CampaignId { get; private set; }
        public Campaign Campaign { get; private set; }
        public Guid ClientId { get; private set; }
        public Client Client { get; private set; }
        public string IdMensagemBemChat { get; private set; } = string.Empty;
        public MessageStatus Status { get; private set; } = MessageStatus.Sent;
        public DateTime DataEnvio { get; private set; }
        public DateTime? DataEntrega { get; private set; }
        public DateTime? DataLeitura { get; private set; }
        public DateTime? DataFalha { get; private set; }
        public string? WebhookPayload { get; private set; }
        public string? MensagemErro { get; private set; }

        private CampaignMessage() { }

        public CampaignMessage(Guid campaignId, Guid clientId)
        {
            if (campaignId == Guid.Empty)
                throw new DomainException("O ID da campanha é obrigatório.");

            if (clientId == Guid.Empty)
                throw new DomainException("O ID do cliente é obrigatório.");

            Id = Guid.NewGuid();
            CampaignId = campaignId;
            ClientId = clientId;
            Status = MessageStatus.Sent;
            DataEnvio = DateTime.UtcNow;

            AddDomainEvent(new MessageCreatedEvent(
                Id,
                CampaignId,
                ClientId,
                DataEnvio
            ));
        }

        public void MarkAsDelivered(string idMensagemBemChat, string? webhookPayload = null)
        {
            if (Status == MessageStatus.Failed)
                throw new DomainException("Não é possível marcar como entregue uma mensagem que falhou.");

            if (Status == MessageStatus.Delivered || Status == MessageStatus.Read)
                throw new DomainException($"A mensagem já está com status '{Status}'.");

            if (string.IsNullOrWhiteSpace(idMensagemBemChat))
                throw new DomainException("O ID da mensagem BemChat é obrigatório.");

            IdMensagemBemChat = idMensagemBemChat;
            Status = MessageStatus.Delivered;
            DataEntrega = DateTime.UtcNow;
            WebhookPayload = webhookPayload;

            AddDomainEvent(new MessageDeliveredEvent(
                Id,
                CampaignId,
                ClientId,
                idMensagemBemChat,
                DataEntrega.Value
            ));
        }
        public void MarkAsFailed(string mensagemErro, string? webhookPayload = null)
        {
            if (string.IsNullOrWhiteSpace(mensagemErro))
                throw new ArgumentNullException(nameof(mensagemErro));

            if (Status == MessageStatus.Read)
                throw new DomainException("Não é possível marcar como falha uma mensagem já lida.");

            Status = MessageStatus.Failed;
            DataFalha = DateTime.UtcNow;
            MensagemErro = mensagemErro;
            WebhookPayload = webhookPayload;

            AddDomainEvent(new MessageFailedEvent(
                Id,
                CampaignId,
                ClientId,
                mensagemErro,
                DataFalha.Value
            ));
        }

        public void AtualizarWebhookPayload(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
                throw new ArgumentNullException(nameof(payload));

            WebhookPayload = payload;
        }

        public bool WasDelivered() => Status == MessageStatus.Delivered || Status == MessageStatus.Read;

        public bool WasRead() => Status == MessageStatus.Read;

        public bool Failed() => Status == MessageStatus.Failed;

        public TimeSpan? GetTimeUntilDelivered() =>
            DataEntrega.HasValue
                ? DataEntrega.Value - DataEnvio
                : null;

        public TimeSpan? GetTimeUntilRead() =>
            DataLeitura.HasValue
                ? DataLeitura.Value - DataEnvio
                : null;

        public bool WaitingDelivery() =>
            Status == MessageStatus.Sent &&
            !DataEntrega.HasValue &&
            DateTime.UtcNow - DataEnvio > TimeSpan.FromMinutes(5);


    }
}
