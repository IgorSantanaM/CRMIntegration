namespace CRMIntegration.Services.CobMais.DTOs.Requests
{
    public record UpdatePhoneRequest(int Id,
        string? Tipo,
        string? Numero,
        string? Ramal,
        string? Observacao,
        bool Ativo,
        bool Contato,
        bool Whatsapp);
}
