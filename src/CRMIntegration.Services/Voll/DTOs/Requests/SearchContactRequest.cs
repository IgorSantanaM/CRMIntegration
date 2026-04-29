using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs.Requests
{
    public record SearchContactRequest(string SearchField, 
        string SearchValue,
        string Condition = "equal to",
        string? Fields = null);
}
