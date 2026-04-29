namespace CRMIntegration.Services.CobMais.DTOs.Responses
{
    public record CobMaisPhoneUpdateResponse(int Id,
        string? Tipo,
        string? Numero,
        bool Ativo,
        bool? Contato,
        bool? Whatsapp);
}
