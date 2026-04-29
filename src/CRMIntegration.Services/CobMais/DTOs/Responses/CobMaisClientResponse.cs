namespace CRMIntegration.Services.CobMais.DTOs.Responses
{
    public record CobMaisClientResponse(int IdPessoa,
        string? ClienteRg,
        string? DataNascimento,
        string CpfCnpj,
        string Nome,
        string? Codigo,
        List<CobMaisPhoneResponse> Telefones,
        List<CobMaisEmailResponse> Emails,
        List<CobMaisAddressResponse> Enderecos);
}
