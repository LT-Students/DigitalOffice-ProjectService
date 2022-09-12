using System;
using System.Collections.Generic;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker.Consumer;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Extensions;
using LT.DigitalOffice.Kernel.BrokerSupport.Middlewares.Token;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers;
using LT.DigitalOffice.ProjectService.Broker;
using LT.DigitalOffice.ProjectService.Broker.Consumers;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using LT.DigitalOffice.Kernel.EFSupport.Extensions;
using LT.DigitalOffice.Kernel.EFSupport.Helpers;
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
using Serilog;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http;
using DigitalOffice.Kernel.RedisSupport.Extensions;

namespace LT.DigitalOffice.ProjectService
{
  public class Startup : BaseApiInfo
  {
    public const string CorsPolicyName = "LtDoCorsPolicy";
    private string redisConnStr;

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

      Version = "1.2.3.3";
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

      if (int.TryParse(Environment.GetEnvironmentVariable("RedisCacheLiveInMinutes"), out int redisCacheLifeTime))
      {
        services.Configure<RedisConfig>(options =>
        {
          options.CacheLiveInMinutes = redisCacheLifeTime;
        });
      }
      else
      {
        services.Configure<RedisConfig>(Configuration.GetSection(RedisConfig.SectionName));
      }

      services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
      services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
      services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));

      services.AddHttpContextAccessor();

      services.AddBusinessObjects();

      string connStr = ConnectionStringHandler.Get(Configuration);

      services.AddDbContext<ProjectServiceDbContext>(options =>
      {
        options.UseSqlServer(connStr);
      });

      redisConnStr = services.AddRedisSingleton(Configuration);

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
      app.UpdateDatabase<ProjectServiceDbContext>();

      FlushRedisDbHelper.FlushDatabase(redisConnStr, Cache.Projects);

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

      ResponseCreatorStatic.ResponseCreatorConfigure(app.ApplicationServices.GetService<IHttpContextAccessor>());
    }

    #endregion

    #region private methods

    #region configure masstransit

    private (string username, string password) GetRabbitMqCredentials()
    {
      static string GetString(string envVar, string formAppsettings, string generated, string fieldName)
      {
        string str = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrEmpty(str))
        {
          str = formAppsettings ?? generated;

          Log.Information(
            formAppsettings == null
              ? $"Default RabbitMq {fieldName} was used."
              : $"RabbitMq {fieldName} from appsetings.json was used.");
        }
        else
        {
          Log.Information($"RabbitMq {fieldName} from environment was used.");
        }

        return str;
      }

      return (GetString("RabbitMqUsername", _rabbitMqConfig.Username, $"{_serviceInfoConfig.Name}_{_serviceInfoConfig.Id}", "Username"),
        GetString("RabbitMqPassword", _rabbitMqConfig.Password, _serviceInfoConfig.Id, "Password"));
    }
    private void ConfigureMassTransit(IServiceCollection services)
    {
      (string username, string password) = GetRabbitMqCredentials();

      services.AddMassTransit(busConfigurator =>
      {
        ConfigureConsumers(busConfigurator);

        busConfigurator.UsingRabbitMq((context, cfg) =>
        {
          cfg.Host(_rabbitMqConfig.Host, "/", host =>
          {
              host.Username(username);
              host.Password(password);
          });

          ConfigureEndpoints(context, cfg, _rabbitMqConfig);
        });

        busConfigurator.AddRequestClients(_rabbitMqConfig);
      });

      services.AddMassTransitHostedService();
    }

    private void ConfigureConsumers(IServiceCollectionBusConfigurator x)
    {
      x.AddConsumer<SearchProjectsConsumer>();
      x.AddConsumer<FindParseEntitiesConsumer>();
      x.AddConsumer<GetProjectsUsersConsumer>();
      x.AddConsumer<GetProjectsConsumer>();
      x.AddConsumer<CheckProjectsExistenceConsumer>();
      x.AddConsumer<CheckProjectUsersExistenceConsumer>();
      x.AddConsumer<DisactivateProjectUserConsumer>();
      x.AddConsumer<CheckFilesAccessesConsumer>();
      x.AddConsumer<CreateFilesConsumer>();
      x.AddConsumer<GetProjectUserRoleConsumer>();
    }

    private void ConfigureEndpoints(
      IBusRegistrationContext context,
      IRabbitMqBusFactoryConfigurator cfg,
      RabbitMqConfig rabbitMqConfig)
    {
      cfg.ReceiveEndpoint(rabbitMqConfig.SearchProjectsEndpoint, ep =>
      {
        ep.ConfigureConsumer<SearchProjectsConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.FindParseEntitiesEndpoint, ep =>
      {
        ep.ConfigureConsumer<FindParseEntitiesConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.GetProjectsUsersEndpoint, ep =>
      {
        ep.ConfigureConsumer<GetProjectsUsersConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.DisactivateProjectUserEndpoint, ep =>
      {
        ep.ConfigureConsumer<DisactivateProjectUserConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.GetProjectsEndpoint, ep =>
      {
        ep.ConfigureConsumer<GetProjectsConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.CheckProjectsExistenceEndpoint, ep =>
      {
        ep.ConfigureConsumer<CheckProjectsExistenceConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.CheckProjectUsersExistenceEndpoint, ep =>
      {
        ep.ConfigureConsumer<CheckProjectUsersExistenceConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.CheckFilesAccessesEndpoint, ep =>
      {
        ep.ConfigureConsumer<CheckFilesAccessesConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.CreateFilesEndpoint, ep =>
      {
        ep.ConfigureConsumer<CreateFilesConsumer>(context);
      });

      cfg.ReceiveEndpoint(rabbitMqConfig.GetProjectUserRoleEndpoint, ep =>
      {
        ep.ConfigureConsumer<GetProjectUserRoleConsumer>(context);
      });
    }

    #endregion

    #endregion
  }
}
