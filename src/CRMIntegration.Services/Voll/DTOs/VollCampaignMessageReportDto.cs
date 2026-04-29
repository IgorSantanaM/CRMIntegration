namespace CRMIntegration.Services.Voll.DTOs.Responses
{
    public record VollCampaignMessageReportDto(string MessageId,
        string To,
        string Status,
        DateTime? SentAt,
        DateTime? DeliveredAt,
        DateTime? ReadAt,
        string? ErrorMessage);
}
