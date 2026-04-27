using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Campaings.Enum
{
    public enum CampaignStatus
    {
        Created = 1,       
        Processing = 2,  
        Sent = 3,      
        Finished = 4,
        Canceled = 5,
        Error = 6          
    }
}
