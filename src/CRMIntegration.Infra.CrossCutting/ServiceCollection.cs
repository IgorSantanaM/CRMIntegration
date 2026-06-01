using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;
using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Infra.Consumers.Consumers;
using CRMIntegration.Infra.Consumers.Definitions;
using CRMIntegration.Infra.Data.Contexts;
using CRMIntegration.Infra.Data.Interceptors;
using CRMIntegration.Infra.Data.Repositories;
using CRMIntegration.Infra.Services.BemChat;
using CRMIntegration.Infra.Services.CobMais;
using CRMIntegration.Services.BemChat;
using CRMIntegration.Services.CobMais;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRMIntegration.Infra.CrossCutting
{
    public static class ServiceCollection
    {
        public static void RegisterApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(TriggerCampaignCommand).Assembly);
            });

            services.AddValidatorsFromAssemblies(
            [
                typeof(TriggerCampaignCommand).Assembly
            ]);
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IBemChatService, BemChatService>(client =>
            {
                client.BaseAddress = new Uri(configuration["BemChatOptions__Domain"]
                    ?? throw new ArgumentNullException("BemChat url not set!"));

                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient<ICobMaisService, CobMaisService>(client =>
            {
                client.BaseAddress = new Uri(configuration["CobMais__Domain"]
                    ?? throw new ArgumentNullException("CobMais url not set!"));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

        public static void RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<PublishDomainEventsInterceptor>();

            services.AddDbContext<CRMIntegrationContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<PublishDomainEventsInterceptor>();

                options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);

                    npgsqlOptions.CommandTimeout(30);
                });

                options.AddInterceptors(interceptor);
            });

            BemChatOptions bemChatOptions = new();

            configuration.GetSection("BemChatOptions").Bind(bemChatOptions);
            services.AddSingleton(bemChatOptions);

            CobMaisOptions cobMaisOptions = new();

            configuration.GetSection("CobMaisOptions").Bind(cobMaisOptions);
            services.AddSingleton(cobMaisOptions);

            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IUnitOfwork, UnitOfWork>();
        }

        public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var useInMemory = configuration.GetValue<bool>("MassTransit:UseInMemory");

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<TriggerCampaignConsumer, TriggerCampaignConsumerDefinition>();

                cfg.AddEntityFrameworkOutbox<CRMIntegrationContext>(o =>
                {
                    o.UsePostgres();

                    o.QueryDelay = TimeSpan.FromSeconds(10);

                    o.QueryMessageLimit = 50;

                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
                });

                if (useInMemory)
                {
                    cfg.UsingInMemory((context, config) =>
                    {
                        config.ConfigureEndpoints(context);
                    });
                }
                else
                {
                    cfg.UsingRabbitMq((context, config) =>
                    {
                        var rabbitMqConnection = configuration.GetConnectionString("RabbitMQ");

                        config.Host(rabbitMqConnection);

                        config.UseRawJsonSerializer();

                        config.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                        config.ConfigureEndpoints(context);
                    });
                }
            });
        }
    }
}
