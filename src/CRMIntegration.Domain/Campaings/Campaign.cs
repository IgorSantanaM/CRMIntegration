using CRMIntegration.Domain.Campaings.Enum;
using CRMIntegration.Domain.Campaings.Events;
using CRMIntegration.Domain.Core.Exceptions;
using CRMIntegration.Domain.Core.Model;

namespace CRMIntegration.Domain.Campaings
{
    public class Campaign : Entity<Guid>, IAggregateRoot
    {
        public string IdCampanhaBemChat { get; private set; } = string.Empty;
        public string Nome { get; private set; } = string.Empty;
        public string Template { get; private set; } = string.Empty;
        public DateTime DataDisparo { get; private set; }
        public CampaignStatus Status { get; private set; } = default;
        public int TotalContatos { get; private set; } = 0;
        public string ChannelIdBemChat { get; private set; } = string.Empty;
        public DateTime? DataCriacao { get; private set; }
        public DateTime? DataFinalizacao { get; private set; }
        public int TotalEnviados { get; private set; } = 0;
        public int TotalEntregues { get; private set; } = 0;
        public int TotalFalhas { get; private set; } = 0;
        public int TotalLidos { get; private set; } = 0;
        public IReadOnlyCollection<CampaignMessage> Mensagens => _mensagens.AsReadOnly();
        private readonly List<CampaignMessage> _mensagens = new();


        protected Campaign()
        { }

        public Campaign(string nome,
            string template,
            string channelIdBemChat,
            DateTime dataDisparo,
            int totalContatos)
        {
            ValidateName(nome);
            ValidateTemplate(template);
            ValidateChannelId(channelIdBemChat);
            ValidateTotalContacts(totalContatos);
            ValidateSendDate(dataDisparo);

            Id = Guid.NewGuid();
            Nome = nome;
            Template = template;
            ChannelIdBemChat = channelIdBemChat;
            DataDisparo = dataDisparo;
            TotalContatos = totalContatos;
            Status = CampaignStatus.Created;
            DataCriacao = DateTime.UtcNow;

            AddDomainEvent(new CampaignCreatedEvent(
                Id,
                Nome,
                Template,
                TotalContatos,
                DataCriacao
            ));
        }

        public void StartProcessing()
        {
            if (Status != CampaignStatus.Created)
                throw new DomainException($"Não é possível processar uma campanha com status '{Status}'.");

            Status = CampaignStatus.Processing;

            AddDomainEvent(new CampaignProcessingStartedEvent(Id, DateTime.UtcNow));
        }

        public void MarkAsSent(string idCampanhaBemChat)
        {
            if (Status != CampaignStatus.Processing)
                throw new DomainException($"Não é possível marcar como enviada uma campanha com status '{Status}'.");

            if (string.IsNullOrWhiteSpace(idCampanhaBemChat))
                throw new DomainException("O ID da campanha BemChat é obrigatório.");

            IdCampanhaBemChat = idCampanhaBemChat;
            Status = CampaignStatus.Sent;

            AddDomainEvent(new CampaignSentEvent(
                Id,
                IdCampanhaBemChat,
                TotalContatos,
                DateTime.UtcNow
            ));
        }

        public void AddMessage(CampaignMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (message.CampaignId != Id)
                throw new DomainException("A mensagem não pertence a esta campanha.");

            if (_mensagens.Any(m => m.ClientId == message.ClientId))
                throw new DomainException("Já existe uma mensagem para este cliente nesta campanha.");

            _mensagens.Add(message);
        }

        public void UpdateMetrics(int sent, int delivered, int read, int fails)
        {
            if (sent < 0 || delivered < 0 || read < 0 || fails < 0)
                throw new DomainException("As métricas não podem ser negativas.");

            if (sent + fails > TotalContatos)
                throw new DomainException("O total de enviados + falhas não pode exceder o total de contatos.");

            if (delivered > sent)
                throw new DomainException("O total de entregues não pode exceder o total de enviados.");

            if (read > delivered)
                throw new DomainException("O total de lidos não pode exceder o total de entregues.");

            var metricasAnteriores = new
            {
                Enviados = TotalEnviados,
                Entregues = TotalEntregues,
                Lidos = TotalLidos,
                Falhas = TotalFalhas
            };

            TotalEnviados = sent;
            TotalEntregues = delivered;
            TotalLidos = read;
            TotalFalhas = fails;

            AddDomainEvent(new CampaignMetricsUpdatedEvent(
                Id,
                sent,
                delivered,
                read,
                fails,
                DateTime.UtcNow
            ));

            VerifyFinalization();
        }

        public void IncrementDelivered()
        {
            TotalEntregues++;
        }

        public void IncrementRead()
        {
            TotalLidos++;
        }

        public void IncrementFail()
        {
            TotalFalhas++;
            VerifyFinalization();
        }

        public void FinishCampaign()
        {
            if (Status == CampaignStatus.Finished)
                throw new DomainException("A campanha já está finalizada.");

            if (Status == CampaignStatus.Error)
                throw new DomainException("Não é possível finalizar uma campanha com erro.");

            Status = CampaignStatus.Finished;
            DataFinalizacao = DateTime.UtcNow;

            var taxaEntrega = TotalEnviados > 0
                ? (decimal)TotalEntregues / TotalEnviados * 100
                : 0;

            var taxaLeitura = TotalEntregues > 0
                ? (decimal)TotalLidos / TotalEntregues * 100
                : 0;

            var taxaFalha = TotalContatos > 0
                ? (decimal)TotalFalhas / TotalContatos * 100
                : 0;

            AddDomainEvent(new CampaignFinishedEvent(
                Id,
                DataFinalizacao.Value,
                TotalContatos,
                TotalEnviados,
                TotalEntregues,
                TotalLidos,
                TotalFalhas,
                taxaEntrega,
                taxaLeitura,
                taxaFalha
            ));
        }

        public void MarkWithError(string motivoErro)
        {
            if (string.IsNullOrWhiteSpace(motivoErro))
                throw new ArgumentNullException(nameof(motivoErro));

            if (Status == CampaignStatus.Finished)
                throw new DomainException("Não é possível marcar como erro uma campanha já finalizada.");

            Status = CampaignStatus.Error;
            DataFinalizacao = DateTime.UtcNow;

            AddDomainEvent(new CampaignErrorEvent(
                Id,
                motivoErro,
                DateTime.UtcNow
            ));
        }

        public void Cancelled(string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentNullException(nameof(motivo));

            if (Status == CampaignStatus.Finished)
                throw new DomainException("Não é possível cancelar uma campanha já finalizada.");

            if (Status == CampaignStatus.Canceled)
                throw new DomainException("A campanha já está cancelada.");

            Status = CampaignStatus.Canceled;
            DataFinalizacao = DateTime.UtcNow;

            AddDomainEvent(new CampaignCancelledEvent(
                Id,
                motivo,
                DateTime.UtcNow
            ));
        }

        public bool CanBeCancelled() =>
            Status == CampaignStatus.Created ||
            Status == CampaignStatus.Processing;

        public bool IsFinished() =>
            Status == CampaignStatus.Finished ||
            Status == CampaignStatus.Error ||
            Status == CampaignStatus.Canceled;

        public decimal CalculateDeliveryRate() =>
            TotalEnviados > 0 ? (decimal)TotalEntregues / TotalEnviados * 100 : 0;

        public decimal CalculateReadRate() =>
            TotalEntregues > 0 ? (decimal)TotalLidos / TotalEntregues * 100 : 0;

        public decimal CalculateFailureRate() =>
            TotalContatos > 0 ? (decimal)TotalFalhas / TotalContatos * 100 : 0;

        public int GetPendingTotal() =>
            TotalContatos - TotalEnviados - TotalFalhas;

        #region Validations

        private void VerifyFinalization()
        {
            var totalProcessado = TotalEnviados + TotalFalhas;

            if (Status == CampaignStatus.Sent && totalProcessado >= TotalContatos)
                FinishCampaign();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome da campanha é obrigatório.");

            if (name.Length < 3)
                throw new DomainException("O nome da campanha deve ter no mínimo 3 caracteres.");

            if (name.Length > 200)
                throw new DomainException("O nome da campanha deve ter no máximo 200 caracteres.");
        }

        private static void ValidateTemplate(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new DomainException("O template é obrigatório.");

            if (template.Length > 100)
                throw new DomainException("O nome do template deve ter no máximo 100 caracteres.");
        }

        private static void ValidateChannelId(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                throw new DomainException("O Channel ID da BemChat é obrigatório.");
        }

        private static void ValidateTotalContacts(int total)
        {
            if (total <= 0)
                throw new DomainException("A campanha deve ter pelo menos 1 contato.");

            if (total > 10000)
                throw new DomainException("A campanha não pode ter mais de 10.000 contatos por vez.");
        }

        private static void ValidateSendDate(DateTime sendDate)
        {
            if (sendDate < DateTime.UtcNow.AddMinutes(-5))
                throw new DomainException("A data de disparo não pode ser no passado.");
        }
        #endregion
    }
}
