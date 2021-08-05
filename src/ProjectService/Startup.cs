using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.Broker.Consumer;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.ProjectService.Broker;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Mappers.Helpers;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService
{
    public class Startup : BaseApiInfo
    {
        public const string CorsPolicyName = "LtDoCorsPolicy";

        public IConfiguration Configuration { get; }

        private readonly RabbitMqConfig _rabbitMqConfig;
        private readonly BaseServiceInfoConfig _serviceInfoConfig;

        #region public methods

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _serviceInfoConfig = Configuration
               .GetSection(BaseServiceInfoConfig.SectionName)
               .Get<BaseServiceInfoConfig>();

            _rabbitMqConfig = Configuration
                .GetSection(BaseRabbitMqConfig.SectionName)
                .Get<RabbitMqConfig>();

            Version = "1.2.1";
            Description = "ProjectService is an API intended to work with projects.";
            StartTime = DateTime.UtcNow;
            ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    CorsPolicyName,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
            services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
            services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));

            services.AddHttpContextAccessor();

            services.AddBusinessObjects();

            string connStr = Environment.GetEnvironmentVariable("ConnectionString");
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = Configuration.GetConnectionString("SQLConnectionString");
            }

            services.AddDbContext<ProjectServiceDbContext>(options =>
            {
                options.UseSqlServer(connStr);
            });

            services.AddControllers();
            services
                .AddHealthChecks()
                .AddSqlServer(connStr)
                .AddRabbitMqCheck();

            ConfigureMassTransit(services);

            services
                .AddControllersWithViews()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            UpdateDatabase(app);

            app.UseForwardedHeaders();

            app.UseExceptionsHandler(loggerFactory);

            app.UseApiInformation();

            app.UseRouting();

            app.UseMiddleware<TokenMiddleware>();

            app.UseCors(CorsPolicyName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(CorsPolicyName);

                endpoints.MapHealthChecks($"/{_serviceInfoConfig.Id}/hc", new HealthCheckOptions
                {
                    ResultStatusCodes = new Dictionary<HealthStatus, int>
                    {
                        { HealthStatus.Unhealthy, 200 },
                        { HealthStatus.Healthy, 200 },
                        { HealthStatus.Degraded, 200 },
                    },
                    Predicate = check => check.Name != "masstransit-bus",
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        #endregion

        #region private methods

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<ProjectServiceDbContext>();

            context.Database.Migrate();
            TaskNumberHelper.LoadCache(context);
        }

        #region configure masstransit

        private void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(busConfigurator =>
            {
                ConfigureConsumers(busConfigurator);

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(_rabbitMqConfig.Host, "/", host =>
                    {
                        host.Username($"{_serviceInfoConfig.Name}_{_serviceInfoConfig.Id}");
                        host.Password(_serviceInfoConfig.Id);
                    });

                    ConfigureEndpoints(context, cfg, _rabbitMqConfig);
                });

                busConfigurator.AddRequestClients(_rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
        }

        private void ConfigureConsumers(IServiceCollectionBusConfigurator x)
        {
            x.AddConsumer<GetProjectIdsConsumer>();
            x.AddConsumer<GetProjectInfoConsumer>();
            x.AddConsumer<GetUserProjectsInfoConsumer>();
            x.AddConsumer<SearchProjectsConsumer>();
            x.AddConsumer<GetDepartmentProjectsConsumer>();
            x.AddConsumer<FindProjectsConsumer>();
            x.AddConsumer<FindParseEntitiesConsumer>();
            x.AddConsumer<DisactivateUserConsumer>();
        }

        private void ConfigureEndpoints(
            IBusRegistrationContext context,
            IRabbitMqBusFactoryConfigurator cfg,
            RabbitMqConfig rabbitMqConfig)
        {
            cfg.ReceiveEndpoint(rabbitMqConfig.GetProjectIdsEndpoint, ep =>
            {
                ep.ConfigureConsumer<GetProjectIdsConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.GetProjectInfoEndpoint, ep =>
            {
                ep.ConfigureConsumer<GetProjectInfoConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.GetUserProjectsInfoEndpoint, ep =>
            {
                ep.ConfigureConsumer<GetUserProjectsInfoConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.SearchProjectsEndpoint, ep =>
            {
                ep.ConfigureConsumer<SearchProjectsConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.GetDepartmentProjectsEndpoint, ep =>
            {
                ep.ConfigureConsumer<GetDepartmentProjectsConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.FindProjectsEndpoint, ep =>
            {
                ep.ConfigureConsumer<FindProjectsConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.FindParseEntitiesEndpoint, ep =>
            {
                ep.ConfigureConsumer<FindParseEntitiesConsumer>(context);
            });

            cfg.ReceiveEndpoint(rabbitMqConfig.DisactivateUserEndpoint, ep =>
            {
                ep.ConfigureConsumer<DisactivateUserConsumer>(context);
            });
        }

        #endregion

        #endregion
    }
}