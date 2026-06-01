using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.BemChat.DTOs.Responses
{
    public record BemChatSendMessageResponse(
        bool Success,
        string? MessageId,
        string? Error
    );
}
