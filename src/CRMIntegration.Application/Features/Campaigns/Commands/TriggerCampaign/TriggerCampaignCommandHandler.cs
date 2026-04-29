using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.CobMais.DTOs.Requests;
using EasyNetQ;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign
{
    public class TriggerCampaignCommandHandler(ICobMaisService cobMaisService, ILogger<TriggerCampaignCommandHandler> logger) // : IRequestHandler<TriggerCampaignCommand, bool>
    {
        //public async Task<bool> Handle(TriggerCampaignCommand request, CancellationToken cancellationToken)
        //{
        //    var getActionableContacts = new GetActionableContactsRequest(request.StartDate, request.EndDate);

        //    var contacts = await cobMaisService.GetActionableContactsAsync(getActionableContacts, cancellationToken);

        //    var events = contacts.SelectMany(c => c.PhoneNumber.Select(p => new ));
        //}
    }
}
