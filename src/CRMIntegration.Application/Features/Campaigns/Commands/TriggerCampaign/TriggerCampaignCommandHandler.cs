using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.CobMais.DTOs.Requests;
using EasyNetQ;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign
{
    public class TriggerCampaignCommandHandler(IPublishEndpoint publishEndpoint)  : IRequestHandler<TriggerCampaignCommand>
    {
        public async Task Handle(TriggerCampaignCommand request, CancellationToken cancellationToken)
        {

            // TODO: add all the contacts and send the events
            await publishEndpoint.Publish(request, cancellationToken);
        }
    }
}
