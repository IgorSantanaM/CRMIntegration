namespace CRMIntegration.Services.Voll.DTOs.Responses
{
    public record VollCampaignResponse(string Id,
        string? Title,
        string? Status,
        int? TotalReceivers,
        DateTime? CreatedAt);
}
