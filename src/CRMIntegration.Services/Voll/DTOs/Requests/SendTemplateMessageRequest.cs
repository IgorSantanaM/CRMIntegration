using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs.Requests
{
    public record SendTemplateMessageRequest(string To,
        CampaignTemplateDto Template,
        string RecipientType = "individual",
        string Type = "template", 
        string? Schedule = null);
}
