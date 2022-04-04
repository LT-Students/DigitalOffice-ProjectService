using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Requests.Time;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class TimeService : ITimeService
  {
    private readonly IRequestClient<ICreateWorkTimeRequest> _rcCreateWorkTime;
    private readonly ILogger<TimeService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public TimeService(
      IRequestClient<ICreateWorkTimeRequest> rcCreateWorkTime,
      ILogger<TimeService> logger,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _rcCreateWorkTime = rcCreateWorkTime;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task CreateWorkTimeAsync(Guid projectId, List<Guid> userIds, List<string> errors)
    {
      await RequestHandler.ProcessRequest<ICreateWorkTimeRequest, bool>(
        _rcCreateWorkTime,
        ICreateWorkTimeRequest.CreateObj(projectId, userIds),
        errors,
        _logger);
    }
  }
}
