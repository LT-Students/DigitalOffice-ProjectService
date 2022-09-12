using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class UserService : IUserService
  {
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUsersExistence;
    private readonly IRequestClient<IFilteredUsersDataRequest> _rcGetFilteredUsers;
    private readonly ILogger<UserService> _logger;
    private readonly IGlobalCacheRepository _globalCache;

    public UserService(
      IRequestClient<ICheckUsersExistence> rcCheckUsersExistence,
      IRequestClient<IFilteredUsersDataRequest> rcGetFilteredUsers,
      ILogger<UserService> logger,
      IGlobalCacheRepository globalCache)
    {
      _rcCheckUsersExistence = rcCheckUsersExistence;
      _rcGetFilteredUsers = rcGetFilteredUsers;
      _logger = logger;
      _globalCache = globalCache;
    }

    public async Task<(List<UserData> usersData, int totalCount)> GetFilteredUsersAsync(List<Guid> usersIds, FindProjectUsersFilter filter)
    {
      if (usersIds is null || !usersIds.Any() || filter is null)
      {
        return default;
      }

      object request = IFilteredUsersDataRequest.CreateObj(
        usersIds: usersIds,
        skipCount: filter.SkipCount,
        takeCount: filter.TakeCount,
        ascendingSort: filter.AscendingSort);

      (List<UserData> usersData, int totalCount) usersFilteredData =
        await _globalCache.GetAsync<(List<UserData>, int)>(Cache.Users, usersIds.GetRedisCacheKey(request.GetBasicProperties()));

      if (usersFilteredData.usersData is not null)
      {
        _logger.LogInformation(
          "UsersDatas were taken from the cache. Users ids: {usersIds}", string.Join(", ", usersIds));
      }
      else
      {
        IFilteredUsersDataResponse response = await _rcGetFilteredUsers.ProcessRequest<IFilteredUsersDataRequest, IFilteredUsersDataResponse>(
          request: request,
          logger: _logger);

        usersFilteredData.usersData = response?.UsersData;
        usersFilteredData.totalCount = response?.TotalCount ?? 0;
      }

      return usersFilteredData;
    }

    public async Task<List<Guid>> CheckUsersExistenceAsync(List<Guid> usersIds, List<string> errors = null)
    {
      if (usersIds is null || !usersIds.Any())
      {
        return null;
      }

      usersIds = (await RequestHandler.ProcessRequest<ICheckUsersExistence, ICheckUsersExistence>(
        _rcCheckUsersExistence,
        ICheckUsersExistence.CreateObj(usersIds),
        errors,
        _logger))?.UserIds;

      return usersIds;
    }
  }
}
