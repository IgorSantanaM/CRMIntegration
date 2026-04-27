using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
   public record ClientMarkedAsActionableEvent(Guid Id, int IdCobMais, string Whatsapp, DateTime Value) : Event<Guid>(Id);
}