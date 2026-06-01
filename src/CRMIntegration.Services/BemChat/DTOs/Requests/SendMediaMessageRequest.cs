using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.BemChat.DTOs.Requests
{
    public record SendMediaMessageRequest(
       string Number,
       Stream MediaStream,
       string FileName,
       string ContentType,
       string? Body = null,
       int? WhatsAppId = null
   );
}
