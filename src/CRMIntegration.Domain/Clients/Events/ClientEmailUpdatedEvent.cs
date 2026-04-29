using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientEmailUpdatedEvent(Guid Id, string? EmailAnterior, string? EmailAtual, DateTime Value) : Event<Guid>(Id);
}
