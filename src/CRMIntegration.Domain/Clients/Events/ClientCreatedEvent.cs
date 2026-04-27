using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientCreatedEvent(Guid Id, int IdCobMais, string Nome, string Whatsapp, DateTime DataCriacao) : Event<Guid>(Id);
}
