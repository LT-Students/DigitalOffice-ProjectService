using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel;
using LT.DigitalOffice.Kernel.Broker;
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
using LT.DigitalOffice.ProjectService.Validation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
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
            services.AddHealthChecks();

            services.AddDbContext<ProjectServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SQLConnectionString"));
            });

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
                    cfg.Host("localhost", "/", host =>
                    {
                        host.Username($"{rabbitMqConfig.Username}_{rabbitMqConfig.Password}");
                        host.Password(rabbitMqConfig.Password);
                    });
                });

                x.AddRequestClient<IGetFileRequest>(new Uri(rabbitMqConfig.FileServiceUrl));
                x.AddRequestClient<IGetUserRequest>(new Uri(rabbitMqConfig.UserServiceUsersUrl), RequestTimeout.After(ms: 100));
                x.AddRequestClient<IGetDepartmentRequest>(new Uri(rabbitMqConfig.CompanyServiceDepartmentsUrl), RequestTimeout.After(ms: 100));

                x.ConfigureKernelMassTransit(rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
	    }

        private void ConfigureCommands(IServiceCollection services)
        {
            services.AddTransient<IGetProjectCommand, GetProjectCommand>();
            services.AddTransient<ICreateNewProjectCommand, CreateNewProjectCommand>();
            services.AddTransient<IEditProjectByIdCommand, EditProjectByIdCommand>();
            services.AddTransient<IDisableWorkersInProjectCommand, DisableWorkersInProjectCommand>();
            services.AddTransient<ICreateRoleCommand, CreateRoleCommand>();
            services.AddTransient<IGetProjectsCommand, GetProjectsCommand>();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddTransient<IProjectRepository, ProjectRepository>();
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
            services.AddTransient<ICreateRoleRequestMapper, CreateRoleRequestMapper>();
            services.AddTransient<IProjectResponseMapper, ProjectResponseMapper>();
            services.AddTransient<IProjectExpandedResponseMapper, ProjectExpandedResponseMapper>();
        }

        private void ConfigureValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<NewProjectRequest>, ProjectValidator>();
            services.AddTransient<IValidator<EditProjectRequest>, EditProjectValidator>();
            services.AddTransient<IValidator<WorkersIdsInProjectRequest>, WorkersProjectIdsValidator>();
            services.AddTransient<IValidator<CreateRoleRequest>, RoleValidator>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/api/healthcheck");

            app.UseExceptionHandler(tempApp => tempApp.Run(CustomExceptionHandler.HandleCustomException));

            UpdateDatabase(app);

            //app.UseMiddleware<TokenMiddleware>();

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