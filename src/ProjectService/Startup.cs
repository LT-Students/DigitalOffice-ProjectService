using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Mappers;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
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

            services.Configure<RabbitMQOptions>(Configuration);

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
            var rabbitmqOptions = Configuration.GetSection(RabbitMQOptions.RabbitMQ).Get<RabbitMQOptions>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", host =>
                    {
                        host.Username($"{rabbitmqOptions.Username}_{rabbitmqOptions.Password}");
                        host.Password(rabbitmqOptions.Password);
                    });
                });

                x.AddRequestClient<IGetFileRequest>(
                  new Uri("rabbitmq://localhost/FileService"));

                x.ConfigureKernelMassTransit(rabbitmqOptions);
            });

            services.AddMassTransitHostedService();
	    }

        private void ConfigureCommands(IServiceCollection services)
        {
            services.AddTransient<IGetProjectInfoByIdCommand, GetProjectByIdCommand>();
            services.AddTransient<ICreateNewProjectCommand, CreateNewProjectCommand>();
            services.AddTransient<IEditProjectByIdCommand, EditProjectByIdCommand>();
            services.AddTransient<IDisableWorkersInProjectCommand, DisableWorkersInProjectCommand>();
            services.AddTransient<IDeleteRoleCommand, DeleteRoleCommand>();
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
            services.AddTransient<IMapper<DbProject, Project>, ProjectMapper>();
            services.AddTransient<IMapper<NewProjectRequest, DbProject>, ProjectMapper>();
            services.AddTransient<IMapper<EditProjectRequest, DbProject>, ProjectMapper>();
        }

        private void ConfigureValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<NewProjectRequest>, ProjectValidator>();
            services.AddTransient<IValidator<EditProjectRequest>, EditProjectValidator>();
            services.AddTransient<IValidator<WorkersIdsInProjectRequest>, WorkersProjectIdsValidator>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHealthChecks("/api/healthcheck");

            app.UseExceptionHandler(tempApp => tempApp.Run(CustomExceptionHandler.HandleCustomException));

            UpdateDatabase(app);

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