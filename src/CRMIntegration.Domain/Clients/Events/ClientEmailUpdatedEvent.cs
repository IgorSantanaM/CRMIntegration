using CRMIntegration.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Clients.Events
{
    public record ClientEmailUpdatedEvent(Guid Id, string? EmailAnterior, string? EmailAtual, DateTime Value) : Event<Guid>(Id);
}
