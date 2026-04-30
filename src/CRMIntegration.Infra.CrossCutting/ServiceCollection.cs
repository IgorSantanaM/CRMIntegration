using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;
using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Clients;
using CRMIntegration.Infra.Consumers.Consumers;
using CRMIntegration.Infra.Data.Contexts;
using CRMIntegration.Infra.Data.Interceptors;
using CRMIntegration.Infra.Data.Repositories;
using CRMIntegration.Infra.Services.CobMais;
using CRMIntegration.Infra.Services.Voll;
using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.Voll;
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

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICobMaisService, CobMaisService>();

            services.AddScoped<IVollService, VollService>();
        }

        public static void RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<PublishDomainEventsInterceptor>();

            services.AddDbContext<CRMIntegrationContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<PublishDomainEventsInterceptor>();

                options.UseNpgsql(configuration.GetConnectionString("DottInDb"), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);

                    npgsqlOptions.CommandTimeout(30);
                });

                options.AddInterceptors(interceptor);
            });

            VollOptions vollOptions = new();

            configuration.GetSection("VollOptions").Bind(vollOptions);
            services.AddSingleton(vollOptions);

            CobMaisOptions cobMaisOptions = new();

            configuration.GetSection("CobMaisOptions").Bind(cobMaisOptions);
            services.AddSingleton(cobMaisOptions);

            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
        }

        public static void AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var useInMemory = configuration.GetValue<bool>("MassTransit:UseInMemory");

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<TriggerCampaignConsumer>();

                cfg.AddEntityFrameworkOutbox<CRMIntegrationContext>(o =>
                {
                    o.UsePostgres();

                    o.QueryDelay = TimeSpan.FromSeconds(10);

                    o.QueryMessageLimit = 50;

                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);

                    o.UseBusOutbox(bo =>
                    {
                        bo.MessageDeliveryLimit = 50;
                    });
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
