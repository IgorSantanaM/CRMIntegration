using CRMIntegration.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record MessageCreatedEvent(Guid Id, Guid CampaignId, Guid ClientId, DateTime SentTime) : Event<Guid>(Id); 
}
