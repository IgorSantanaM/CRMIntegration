using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientWhatsAppUpdatedEvent(Guid Id, string WhatsAppAnterior, string WhatsAppAtual, DateTime Value) : Event<Guid>(Id);
}
