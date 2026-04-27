using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients;

public record ClientMarkedAsNonActionableEvent(Guid Id, int IdCobMais, string Whatsapp, DateTime Value) : Event<Guid>(Id);