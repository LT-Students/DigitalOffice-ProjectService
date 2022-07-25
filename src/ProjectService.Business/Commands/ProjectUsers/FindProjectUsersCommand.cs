﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  public class FindProjectUsersCommand : IFindProjectUsersCommand
  {
    private readonly IBaseFindFilterValidator _baseFindFilterValidator;
    private readonly IProjectUserRepository _projectUserRepository;
    private readonly IUserService _userService;
    private readonly IImageService _imageService;
    private readonly IPositionService _positionService;
    private readonly IUserInfoMapper _userInfoMapper;

    public FindProjectUsersCommand(
      IBaseFindFilterValidator baseFindFilterValidator,
      IProjectUserRepository projectUserRepository,
      IUserService userService,
      IImageService imageService,
      IPositionService positionService,
      IUserInfoMapper userInfoMapper)
    {
      _baseFindFilterValidator = baseFindFilterValidator;
      _projectUserRepository = projectUserRepository;
      _userService = userService;
      _imageService = imageService;
      _positionService = positionService;
      _userInfoMapper = userInfoMapper;
    }

    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(Guid projectId, FindProjectUsersFilter filter)
    {
      if (!_baseFindFilterValidator.ValidateCustom(filter, out List<string> errors))
      {
        return ResponseCreatorStatic.CreateFindResponse<UserInfo>(statusCode: HttpStatusCode.BadRequest, errors: errors);
      }

      List<DbProjectUser> projectUsers = await _projectUserRepository.GetAsync(projectId: projectId, isActive: filter.IsActive);

      if (projectUsers is null)
      {
        return new();
      }

      (List<UserData> usersData, int totalCount) filteredUsersData =
        await _userService.GetFilteredUsersAsync(projectUsers.Select(pu => pu.UserId).ToList(), filter);

      Task<List<ImageInfo>> usersAvatarsTask = filter.IncludeAvatars
        ? _imageService.GetImagesAsync(
            imagesIds: filteredUsersData.usersData?.Where(x => x.ImageId.HasValue).Select(x => x.ImageId.Value).ToList(),
            imageSource: ImageSource.User)
        : Task.FromResult<List<ImageInfo>>(default);

      Task<List<PositionData>> usersPositionsTask = filter.IncludePositions
        ? _positionService.GetPositionsAsync(usersIds: filteredUsersData.usersData?.Select(x => x.Id).ToList())
        : Task.FromResult<List<PositionData>>(default);

      List<ImageInfo> usersAvatars = await usersAvatarsTask;
      List<PositionData> usersPositions = await usersPositionsTask;

      return new()
      {
        Body = filteredUsersData.usersData?.Select(userData =>
          _userInfoMapper.Map(
            dbProjectUser: projectUsers.FirstOrDefault(pu => pu.UserId == userData.Id),
            userData: userData,
            image: usersAvatars?.FirstOrDefault(av => av.Id == userData.ImageId),
            userPosition: usersPositions?.FirstOrDefault(p => p.UsersIds.Contains(userData.Id)))
          ).ToList(),

        TotalCount = filteredUsersData.totalCount
      };
    }
  }
}