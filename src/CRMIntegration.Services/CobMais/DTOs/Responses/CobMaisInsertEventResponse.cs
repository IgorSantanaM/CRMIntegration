using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.CobMais.DTOs.Responses
{
    public record CobMaisInsertEventResponse(int? Id, bool Success, string? Message);
}
