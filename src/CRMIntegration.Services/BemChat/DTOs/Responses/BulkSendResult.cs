using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.BemChat.DTOs.Responses
{
    public record BulkSendResult(
        string Number,
        string Name,
        bool Success,
        string? MessageId,
        string? Error
    );
}
