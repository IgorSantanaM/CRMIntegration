namespace CRMIntegration.Services.Voll.DTOs.Responses
{
    public record VollSendMessageResponse(string MessageId, bool Success, string? Error);
}
