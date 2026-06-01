using CRMIntegration.Domain.Core.Events;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientSynchronizedBemChatEvent(Guid Id,
        int IdCobMais,
        string IdBemChat,
        DateTime DataSincronizacaoVoll) : Event<Guid>(Id);
}
