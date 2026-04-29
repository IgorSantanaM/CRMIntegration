using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs
{
    public record CampaignTemplateDto(CampaignTemplateLanguage Language, 
        string Name,
        List<CampaignTemplateComponentDto> Components);
}
