using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Application.Features.Campaigns.DTOs
{
    public record TriggerCampaignFromCsvResponse(
        Guid CampaignId,
        int TotalContacts,
        int Skipped,
        string Message
    );
}
