using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs
{
    public record CampaignTemplateParameterDto(string Type = "text", string? Text = null);
}
