using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.CobMais.DTOs.Responses
{
    public record CobMaisEmailResponse(int Id, string? Email, string? Observacao, bool Ativo);
}
