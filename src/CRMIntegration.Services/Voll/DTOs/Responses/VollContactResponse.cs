using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs.Responses
{
    public record VollContactResponse(string Id,
        string? Whatsapp,
        string? ChatName,
        string? Email,
        string? ChatOperator,
        List<VollCustomFieldValueDto> CustomFields);
}
