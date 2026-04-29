namespace CRMIntegration.Services.Voll.DTOs.Responses
{
    public record VollCampaignReportResponse(string CampaignId,
        string? Title,
        string? Status,
        int TotalSent,
        int TotalDelivered,
        int TotalRead,
        int TotalFailed,
        List<VollCampaignMessageReportDto> Messages);
}
