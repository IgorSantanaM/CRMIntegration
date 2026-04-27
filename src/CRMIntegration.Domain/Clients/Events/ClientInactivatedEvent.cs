using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events;

public record ClientInactivatedEvent(Guid Id, int IdCobMais, string Motivo, DateTime Value) : Event<Guid>(Id);
