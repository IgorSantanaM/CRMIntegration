using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientActivatedEvent(Guid Id, int IdCobMais, DateTime Value) : Event<Guid>(Id);
}
