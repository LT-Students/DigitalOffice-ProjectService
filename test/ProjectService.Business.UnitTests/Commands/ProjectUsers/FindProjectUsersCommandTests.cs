using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands.ProjectUsers
{
  internal class FindProjectUsersCommandTests
  {
    private IFindProjectUsersCommand _command;
    private AutoMocker _mocker;

    private List<DbProjectUser> _dbProjectUsers;
    private List<UserData> _usersData;
    private List<UserInfo> _usersInfo;
    private UserInfo _userInfo;
    private FindProjectUsersFilter _filter;
    private Guid _projectId;
    private CancellationToken _cancellationToken;

    private void Verifiable(
      Times projectRepositoryTimes,
      Times userServiceTimes,
      Times imageServiceTimes,
      Times positionServiceTimes,
      Times userInfoMapperTimes)
    {
      _mocker.Verify<IProjectUserRepository, Task<List<DbProjectUser>>>(x => x.GetAsync(_projectId, _filter.IsActive, _cancellationToken), projectRepositoryTimes);
      _mocker.Verify<IUserService, Task<(List<UserData> usersData, int totalCount)>>(x =>
          x.GetFilteredUsersAsync(It.IsAny<List<Guid>>(), _filter, _cancellationToken), userServiceTimes);
      _mocker.Verify<IImageService, Task<List<ImageInfo>>>(x =>
        x.GetImagesAsync(It.IsAny<List<Guid>>(), ImageSource.User, default, _cancellationToken), imageServiceTimes);
      _mocker.Verify<IPositionService, Task<List<PositionData>>>(x => x.GetPositionsAsync(It.IsAny<List<Guid>>(), default, _cancellationToken), positionServiceTimes);
      _mocker.Verify<IUserInfoMapper, UserInfo>(x => x.Map(
          It.IsAny<DbProjectUser>(),
          It.IsAny<UserData>(),
          It.IsAny<ImageInfo>(),
          It.IsAny<PositionData>()), userInfoMapperTimes);

      _mocker.Resolvers.Clear();
    }

    #region Setup

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<FindProjectUsersCommand>();

      _filter = new FindProjectUsersFilter
      {
        IsActive = true,
        IncludeAvatars = true,
        IncludePositions = true
      };

      _projectId = Guid.NewGuid();

      _dbProjectUsers = new List<DbProjectUser>
      {
        new DbProjectUser
        {
          Id = Guid.NewGuid()
        },
        new DbProjectUser
        {
          Id = Guid.NewGuid()
        }
      };

      _usersData = new List<UserData>
      {
        new UserData(Guid.NewGuid(), Guid.NewGuid(), "firstName", "middleName", "lastName", true)
      };

      _userInfo = new UserInfo
      {
        Id = Guid.NewGuid(),
        FirstName = "firstName",
        MiddleName = "middleName",
        LastName = "lastName"
      };

      _usersInfo = new List<UserInfo>
      {
        _userInfo
      };
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IProjectUserRepository>().Reset();
      _mocker.GetMock<IUserService>().Reset();
      _mocker.GetMock<IImageService>().Reset();
      _mocker.GetMock<IPositionService>().Reset();
      _mocker.GetMock<IUserInfoMapper>().Reset();
    }

    #endregion

    [Test]
    public async Task ProjectUsersIsNull()
    {
      _mocker
        .Setup<IProjectUserRepository, Task<List<DbProjectUser>>>(x => x.GetAsync(_projectId, _filter.IsActive, _cancellationToken))
        .ReturnsAsync(It.IsAny<List<DbProjectUser>>());

      FindResultResponse<ProjectInfo> expectedResponse = new();

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _filter, _cancellationToken));

      Verifiable(
        projectRepositoryTimes: Times.Once(),
        userServiceTimes: Times.Never(),
        imageServiceTimes: Times.Never(),
        positionServiceTimes: Times.Never(),
        userInfoMapperTimes: Times.Never());
    }

    //TODO fix it and add ByPositions
    /*[Test]
   public async Task SuccessResult()
   {
     _mocker
       .Setup<IProjectUserRepository, Task<List<DbProjectUser>>>(x => x.GetAsync(_projectId, _filter.IsActive, _cancellationToken))
       .ReturnsAsync(_dbProjectUsers);

     _mocker
       .Setup<IUserService, Task<(List<UserData> usersData, int totalCount)>>(x =>
         x.GetFilteredUsersAsync(It.IsAny<List<Guid>>(), _filter, _cancellationToken))
       .ReturnsAsync((_usersData, 1));

     _mocker
       .Setup<IImageService, Task<List<ImageInfo>>>(x => x.GetImagesAsync(It.IsAny<List<Guid>>(), ImageSource.User, default, default))
       .ReturnsAsync(It.IsAny<List<ImageInfo>>());

     _mocker
       .Setup<IPositionService, Task<List<PositionData>>>(x => x.GetPositionsAsync(It.IsAny<List<Guid>>(), default, _cancellationToken))
       .ReturnsAsync(It.IsAny<List<PositionData>>());

     _mocker
       .Setup<IUserInfoMapper, UserInfo>(x => x.Map(
         It.IsAny<DbProjectUser>(),
         It.IsAny<UserData>(),
         It.IsAny<ImageInfo>(),
         It.IsAny<PositionData>()))
       .Returns(_userInfo);

     FindResultResponse<UserInfo> expectedResponse = new()
     {
       TotalCount = 1,
       Body = _usersInfo
     };

     SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _filter, _cancellationToken));

     Verifiable(
       projectRepositoryTimes: Times.Once(),
       userServiceTimes: Times.Once(),
       imageServiceTimes: Times.Once(),
       positionServiceTimes: Times.Once(),
       userInfoMapperTimes: Times.Once());
   }*/
  }
}
