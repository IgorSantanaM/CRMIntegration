namespace CRMIntegration.Services.Voll.DTOs.Responses
{
    public record VollTemplateResponse(string Id,
        string Name,
        string? Language,
        string? Category,
        string? Status,
        List<object> Components);
}
