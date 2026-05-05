using CRMIntegration.Infra.Consumers.Consumers;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Infra.Consumers.Definitions
{
    public class TriggerCampaignConsumerDefinition : ConsumerDefinition<TriggerCampaignConsumer>
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<TriggerCampaignConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            endpointConfigurator.UseRateLimit(80, TimeSpan.FromSeconds(1));
        }
    }
}
