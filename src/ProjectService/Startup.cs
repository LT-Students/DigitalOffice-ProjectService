using FluentValidation;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Validation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;

namespace LT.DigitalOffice.ProjectService
{
    public class Startup : BaseApiInfo
    {
        public IConfiguration Configuration { get; }

        private readonly RabbitMqConfig _rabbitMqConfig;
        private readonly BaseServiceInfoConfig _serviceInfoConfig;
        private readonly ILogger<Startup> _logger;

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

            Version = "1.1.4";
            Description = "ProjectService is an API intended to work with projects.";
            StartTime = DateTime.UtcNow;
            ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("LT.DigitalOffice.ProjectService.Startup", LogLevel.Trace)
                    .AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
            services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
            services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));

            services.AddHttpContextAccessor();

            services.AddBusinessObjects(_logger);

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

            ConfigureValidators(services);
            ConfigureMassTransit(services);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            UpdateDatabase(app);

            app.UseExceptionsHandler(loggerFactory);

            app.UseMiddleware<TokenMiddleware>();

            app.UseApiInformation();

#if RELEASE
            app.UseHttpsRedirection();
#endif

            app.UseRouting();

            string corsUrl = Configuration.GetSection("Settings")["CorsUrl"];

            app.UseCors(builder =>
                builder
                    .WithOrigins(corsUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

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
        }

        #region configure masstransit

        private void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(_rabbitMqConfig.Host, "/", host =>
                    {
                        host.Username($"{_serviceInfoConfig.Name}_{_serviceInfoConfig.Id}");
                        host.Password(_serviceInfoConfig.Id);
                    });
                });

                busConfigurator.AddRequestClients(_rabbitMqConfig, _logger);
            });

            services.AddMassTransitHostedService();
        }

        #endregion

        #endregion

        private void ConfigureValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<AddUsersToProjectRequest>, AddUsersToProjectValidator>();
            services.AddTransient<IValidator<ProjectExpandedRequest>, ProjectExpandedValidator>();
            services.AddTransient<IValidator<EditProjectRequest>, EditProjectValidator>();
            services.AddTransient<IValidator<ProjectUserRequest>, ProjectUserValidator>();
        }
    }
}