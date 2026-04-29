using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.Voll.DTOs.Requests
{
    public record UpdateCustomFieldRequest(List<VollCustomFieldValueDto> Data);

}
