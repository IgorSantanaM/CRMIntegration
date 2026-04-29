using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientNameUpdatedEvent(Guid Id, string NomeAnterior, string NomeAtual, DateTime Value) : Event<Guid>(Id);
}
