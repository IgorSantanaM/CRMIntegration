using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientSynchronizedVollEvent(Guid Id,
        int IdCobMais,
        string IdVoll,
        DateTime DataSincronizacaoVoll) : Event<Guid>(Id);
}
