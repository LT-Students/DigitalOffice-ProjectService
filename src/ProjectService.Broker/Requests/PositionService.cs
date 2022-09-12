using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class PositionService : IPositionService
  {
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositions;
    private readonly ILogger<PositionService> _logger;
    private readonly IGlobalCacheRepository _globalCache;

    public PositionService(
      IRequestClient<IGetPositionsRequest> rcGetPositions,
      ILogger<PositionService> logger,
      IGlobalCacheRepository globalCache)
    {
      _rcGetPositions = rcGetPositions;
      _logger = logger;
      _globalCache = globalCache;
    }

    public async Task<List<PositionData>> GetPositionsAsync(
      List<Guid> usersIds,
      List<string> errors = null)
    {
      if (usersIds is null)
      {
        return null;
      }

      object request = IGetPositionsRequest.CreateObj(usersIds);

      List<PositionData> positions = await _globalCache.GetAsync<List<PositionData>>(Cache.Positions, usersIds.GetRedisCacheKey(request.GetBasicProperties()));

      if (positions is not null)
      {
        _logger.LogInformation($"Positions were taken from the cache.");
      }
      else
      {
        positions = (await RequestHandler.ProcessRequest<IGetPositionsRequest, IGetPositionsResponse>(
            _rcGetPositions,
            request,
            errors,
            _logger))
          ?.Positions;
      }

      return positions;
    }
  }
}
