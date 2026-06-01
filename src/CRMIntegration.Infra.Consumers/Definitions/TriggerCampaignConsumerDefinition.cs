using CRMIntegration.Infra.Consumers.Consumers;
using MassTransit;

namespace CRMIntegration.Infra.Consumers.Definitions
{
    public class TriggerCampaignConsumerDefinition : ConsumerDefinition<TriggerCampaignConsumer>
    {
        public TriggerCampaignConsumerDefinition()
        {
            ConcurrentMessageLimit = 10;
        }
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<TriggerCampaignConsumer> consumerConfigurator,
            IRegistrationContext context)
        {
            endpointConfigurator.UseRateLimit(10, TimeSpan.FromSeconds(1));

            endpointConfigurator.UseMessageRetry(r =>
            r.Exponential(3, TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(5)));
        }
    }
}
