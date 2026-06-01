using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.BemChat.DTOs.Responses
{
    public record BemChatBulkCampaignResponse(
       Guid CampaignId,
       string Title,
       int TotalRecipients,
       int Sent,
       int Failed,
       DateTime StartedAt,
       DateTime FinishedAt,
       IReadOnlyList<BulkSendResult> Results
   )
    {
        public decimal SuccessRate =>
            TotalRecipients > 0 ? (decimal)Sent / TotalRecipients * 100 : 0;

        public decimal FailureRate =>
            TotalRecipients > 0 ? (decimal)Failed / TotalRecipients * 100 : 0;
    }
}
