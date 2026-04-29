namespace CRMIntegration.Services.Voll.DTOs.Requests
{
    public record BulkCampaignRequest(string Title,
        string Channel,
        string? Schedule,
        List<CampaignReceiverDto> Receivers,
        List<CampaignMessageDto> Messages,
        string Action = "trigger", 
        string Mode = "immediate");

}
