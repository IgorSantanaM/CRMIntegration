using CRMIntegration.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientNameUpdatedEvent(Guid Id, string NomeAnterior, string NomeAtual, DateTime Value) : Event<Guid>(Id);
}
