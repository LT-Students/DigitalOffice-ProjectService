using FluentValidation;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Configuration;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Validation;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LT.DigitalOffice.ProjectService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connStr = Environment.GetEnvironmentVariable("ConnectionString");
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = Configuration.GetConnectionString("SQLConnectionString");
            }

            Console.WriteLine(connStr);

            services.AddDbContext<ProjectServiceDbContext>(options =>
            {
                options.UseSqlServer(connStr);
            });

            services.AddHealthChecks().AddSqlServer(connStr);
            services.AddControllers();
            services.AddKernelExtensions();

            ConfigureCommands(services);
            ConfigureRepositories(services);
            ConfigureProvider(services);
            ConfigureMappers(services);
            ConfigureValidators(services);
            ConfigureMassTransit(services);
        }

        private void ConfigureMassTransit(IServiceCollection services)
        {
            var rabbitMqConfig = Configuration.GetSection(BaseRabbitMqOptions.RabbitMqSectionName).Get<RabbitMqConfig>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConfig.Host, "/", host =>
                    {
                        host.Username($"{rabbitMqConfig.Username}_{rabbitMqConfig.Password}");
                        host.Password(rabbitMqConfig.Password);
                    });
                });

                RegisterRequestClients(x, rabbitMqConfig);

                x.ConfigureKernelMassTransit(rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
	    }

        private void RegisterRequestClients(
            IServiceCollectionBusConfigurator busConfigurator,
            RabbitMqConfig rabbitMqConfig)
        {
            busConfigurator.AddRequestClient<IGetFileRequest>(
                    new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetFileEndpoint}"));

            busConfigurator.AddRequestClient<IGetUserRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetUserDataEndpoint}"),
                RequestTimeout.After(ms: 100));

            busConfigurator.AddRequestClient<IGetDepartmentRequest>(
                new Uri($"{rabbitMqConfig.BaseUrl}/{rabbitMqConfig.GetDepartmentDataEndpoint}"),
                RequestTimeout.After(ms: 100));
        }

        private void ConfigureCommands(IServiceCollection services)
        {
            services.AddTransient<IGetProjectByIdCommand, GetProjectByIdCommand>();
            //services.AddTransient<ICreateNewProjectCommand, CreateNewProjectCommand>();
            //services.AddTransient<IEditProjectByIdCommand, EditProjectByIdCommand>();

            services.AddTransient<IAddUsersToProjectCommand, AddUsersToProjectCommand>();
            services.AddTransient<IDisableWorkersInProjectCommand, DisableWorkersInProjectCommand>();
            services.AddTransient<IGetProjectsCommand, GetProjectsCommand>();
            services.AddTransient<IDisableRoleCommand, DisableRoleCommand>();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
        }

        private void ConfigureProvider(IServiceCollection services)
        {
            services.AddTransient<IDataProvider, ProjectServiceDbContext>();
        }

        private void ConfigureMappers(IServiceCollection services)
        {
            services.AddTransient<IProjectMapper, ProjectMapper>();
            services.AddTransient<IProjectUserMapper, ProjectUserMapper>();
            services.AddTransient<IRoleMapper, RoleMapper>();

            services.AddTransient<IProjectExpandedRequestMapper, ProjectExpandedRequestMapper>();
            services.AddTransient<IProjectResponseMapper, ProjectResponseMapper>();
            services.AddTransient<IProjectUserRequestMapper, ProjectUserRequestMapper>();
            services.AddTransient<IProjectExpandedResponseMapper, ProjectExpandedResponseMapper>();
    }

        private void ConfigureValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<AddUsersToProjectRequest>, AddUsersToProjectValidator>();
            services.AddTransient<IValidator<ProjectExpandedRequest>, ProjectExpandedRequestValidator>();
            services.AddTransient<IValidator<EditProjectRequest>, EditProjectValidator>();
            services.AddTransient<IValidator<ProjectUserRequest>, ProjectUserRequestValidator>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/api/healthcheck");

            app.UseExceptionHandler(tempApp => tempApp.Run(CustomExceptionHandler.HandleCustomException));

            UpdateDatabase(app);

            app.UseMiddleware<TokenMiddleware>();

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

            var rabbitMqConfig = Configuration
                .GetSection(BaseRabbitMqOptions.RabbitMqSectionName)
                .Get<RabbitMqConfig>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks($"/{rabbitMqConfig.Password}/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            using var context = serviceScope.ServiceProvider.GetService<ProjectServiceDbContext>();

            context.Database.Migrate();
        }
    }
}