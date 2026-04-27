using CRMIntegration.Domain.Core.Model;

namespace CRMIntegration.Domain.Clients
{
    public class Client : Entity<Guid>, IAggregateRoot
    {
        public string IdVoll { get; set; } = string.Empty;
        public int IdCobMais { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string CPFCNPJ { get; set; } = string.Empty;
        public int? IdTelefoneCobMais { get; set; }
        public bool Acionavel { get; set; } = true;
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataUltimoAcionamento { get; set; }
        public DateTime? DataSincronizacaoVoll { get; set; }
        public string? Email { get; set; }
    }
}
