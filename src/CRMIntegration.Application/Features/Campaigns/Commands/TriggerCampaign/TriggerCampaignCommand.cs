using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign
{
    public record TriggerCampaignCommand(DateTime? StartDate, DateTime? EndDate) : IRequest<bool>;
}
