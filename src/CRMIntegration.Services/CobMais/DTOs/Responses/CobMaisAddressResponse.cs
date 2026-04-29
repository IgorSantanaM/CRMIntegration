using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.CobMais.DTOs.Responses
{
    public record CobMaisAddressResponse(int Id,
        string? Tipo,
        string? Logradouro,
        string? Numero,
        string? Complemento,
        string? Bairro,
        string? Cep,
        string? Cidade,
        string? Uf,
        bool Ativo);
}
