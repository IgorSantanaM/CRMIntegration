using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs
{
    public record CampaignMessageDto(CampaignTemplateDto Template, string Type = "template");
}
