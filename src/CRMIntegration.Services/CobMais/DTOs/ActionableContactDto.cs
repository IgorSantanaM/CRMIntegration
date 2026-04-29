using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.CobMais.DTOs
{
    public record ActionableContactDto(int IdPessoa,
        string CpfCnpj,
        string Nome,
        string? Codigo,
        int PhoneId,
        string PhoneNumber,
        bool IsWhatsApp,
        string? Email);
}
