using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs.Requests
{
    public record CreateContactRequest(string Whatsapp, 
        string ChatName, 
        string? Email, 
        string? ChatOperator, 
        List<VollCustomFieldValueDto> CustomFields);
}
