using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs
{
    public record CampaignReceiverDto(string To, Dictionary<string, string> Variables);
    
}
