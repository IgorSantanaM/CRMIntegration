namespace CRMIntegration.Services.Voll.DTOs
{
    public record CampaignTemplateComponentDto(string Type = "body", List<CampaignTemplateParameterDto> Parameters = null);
}
