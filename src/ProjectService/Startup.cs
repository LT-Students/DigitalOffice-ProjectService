using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.Broker.Consumer;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.Middlewares.Token;
using LT.DigitalOffice.ProjectService.Broker;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
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
using Serilog;
using StackExchange.Redis;

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

            Version = "1.2.1.5";
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
      services.AddTransient<IRedisHelper, RedisHelper>();

      string connStr = Environment.GetEnvironmentVariable("ConnectionString");
      if (string.IsNullOrEmpty(connStr))
      {
        connStr = Configuration.GetConnectionString("SQLConnectionString");

        Log.Information($"SQL connection string from appsettings.json was used. Value '{HidePassord(connStr)}'.");
      }
      else
      {
        Log.Information( $"SQL connection string from environment was used. Value '{HidePassord(connStr)}'.");
      }

      services.AddDbContext<ProjectServiceDbContext>(options =>
      {
        options.UseSqlServer(connStr);
      });

      string redisConnStr = Environment.GetEnvironmentVariable("RedisConnectionString");
      if (string.IsNullOrEmpty(redisConnStr))
      {
        redisConnStr = Configuration.GetConnectionString("Redis");

        Log.Information( $"Redis connection string from appsettings.json was used. Value '{HidePassord(redisConnStr)}'");
      }
      else
      {
        Log.Information( $"Redis connection string from environment was used. Value '{HidePassord(redisConnStr)}'");
      }

      services.AddSingleton<IConnectionMultiplexer>(
        x => ConnectionMultiplexer.Connect(redisConnStr));

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
    }

    private string HidePassord(string line)
    {
      string password = "Password";

      int index = line.IndexOf(password, 0, StringComparison.OrdinalIgnoreCase);

      if (index != -1)
      {
        string[] words = Regex.Split(line, @"[=,; ]");

        for (int i = 0; i < words.Length; i++)
        {
          if (string.Equals(password, words[i], StringComparison.OrdinalIgnoreCase))
          {
            line = line.Replace(words[i + 1], "****");
            break;
          }
        }
      }

      return line;
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
      x.AddConsumer<SearchProjectsConsumer>();
      x.AddConsumer<FindParseEntitiesConsumer>();
      x.AddConsumer<GetProjectsUsersConsumer>();
      x.AddConsumer<DisactivateUserConsumer>();
      x.AddConsumer<GetProjectsConsumer>();
      x.AddConsumer<CheckProjectsExistenceConsumer>();
      x.AddConsumer<CheckProjectUsersExistenceConsumer>();
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

      cfg.ReceiveEndpoint(rabbitMqConfig.DisactivateUserEndpoint, ep =>
      {
        ep.ConfigureConsumer<DisactivateUserConsumer>(context);
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
    }

    #endregion

    #endregion
  }
}
