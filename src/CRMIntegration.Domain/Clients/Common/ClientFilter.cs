namespace CRMIntegration.Domain.Clients.Common
{
    public record ClientFilter(
        string? Name = null,
        string? WhatsApp = null,
        string? CPFCNPJ = null,
        string? Email = null,
        bool? Acionavel = null,
        bool? Ativo = null,
        bool? SynchronizedWithVoll = null,
        DateTime? CreatedAfter = null,
        DateTime? CreatedBefore = null,
        DateTime? LastActionAfter = null,
        DateTime? LastActionBefore = null
    );
}
