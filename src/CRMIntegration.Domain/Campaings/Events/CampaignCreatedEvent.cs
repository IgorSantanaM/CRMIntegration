using CRMIntegration.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Campaings.Events
{
    public record CampaignCreatedEvent(Guid Id, 
                                        string Nome,
                                        string Template,
                                        int TotalContatos,
                                        DateTime? DataCriacao) : Event<Guid>(Id);
}
