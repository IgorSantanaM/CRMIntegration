using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.CobMais.DTOs.Responses
{
    public record CobMaisPhoneResponse(int Id,
        string? Tipo, 
        string? Numero, 
        string? Ramal, 
        string? Observacao, 
        bool Ativo, 
        bool? Contato, 
        bool? Whatsapp);
}
