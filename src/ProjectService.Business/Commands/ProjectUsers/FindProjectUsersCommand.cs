using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class FindProjectUsersCommand : IFindProjectUsersCommand
  {
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IUserService _userService;
    private readonly IImageService _imageService;
    private readonly IPositionService _positionService;
    private readonly IUserInfoMapper _userInfoMapper;

    public FindProjectUsersCommand(
      IProjectUserRepository projectUserRepository,
      IUserService userService,
      IImageService imageService,
      IPositionService positionService,
      IUserInfoMapper userInfoMapper)
    {
      _projectUserRepository = projectUserRepository;
      _userService = userService;
      _imageService = imageService;
      _positionService = positionService;
      _userInfoMapper = userInfoMapper;
    }

    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(
      Guid projectId, FindProjectUsersFilter filter, CancellationToken cancellationToken)
    {
      List<DbProjectUser> projectUsers = await _projectUserRepository.GetAsync(
        projectId: projectId, isActive: filter.IsActive, cancellationToken: cancellationToken);

      List<string> errors = new();

      if (projectUsers is null || !projectUsers.Any())
      {
        return new(errors: errors);
      }

      IEnumerable<Guid> usersIds = projectUsers.Select(pu => pu.UserId);

      if (filter.PositionId.HasValue)
      {
        PositionFilteredData data = (await _positionService.GetPositionFilteredDataAsync(
          new List<Guid>() { filter.PositionId.Value }, errors))?.FirstOrDefault();

        usersIds = data is not null ? usersIds.Where(data.UsersIds.Contains).ToList() : Enumerable.Empty<Guid>();
      }

      (List<UserData> usersData, int totalCount) = await _userService.GetFilteredUsersAsync(usersIds.ToList(), filter, cancellationToken);

      Task<List<PositionData>> usersPositionsTask = filter.IncludePositions
        ? _positionService.GetPositionsAsync(usersIds: usersData?.Select(ud => ud.Id).ToList(), errors, cancellationToken)
        : Task.FromResult<List<PositionData>>(default);

      List<PositionData> usersPositions = await usersPositionsTask;

      return new FindResultResponse<UserInfo>(
        errors: errors,
        totalCount: totalCount,
        body: usersData?.Select(userData => _userInfoMapper.Map(
          dbProjectUser: projectUsers.FirstOrDefault(pu => pu.UserId == userData.Id),
          userData: userData,
          userPosition: usersPositions?.FirstOrDefault(up => up.UsersIds.Contains(userData.Id))))
        .ToList());
    }
  }
}
