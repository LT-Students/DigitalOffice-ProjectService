using FluentValidation;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
  internal class CreateProjectUsersCommandTests
  {
    private Guid _authorId;
    private Guid _userId;
    private Guid _projectId;
    private AutoMocker _mocker;
    private List<DbProjectUser> _existingUsers;
    private CreateProjectUsersRequest _request;
    private ICreateProjectUsersCommand _command;
    private UserRequest _userRequest;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times сreateProjectUsersRequestValidatorTimes,
      Times projectUserRepositoryGetTimes,
      Times projectUserRepositoryCreateTimes,
      Times projectUserRepositoryEditTimes,
      Times projectRepositoryTimes,
      Times dbProjectUserMapperTimes,
      Times globalCacheRepositoryTimes,
      Times publishTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<ICreateProjectUsersRequestValidator, bool>(
        x => x.ValidateAsync(It.IsAny<CreateProjectUsersRequest>(), default).Result.IsValid, сreateProjectUsersRequestValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IProjectUserRepository, Task<List<DbProjectUser>>>(x => x.GetExistingUsersAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()), projectUserRepositoryGetTimes);
      _mocker.Verify<IDbProjectUserMapper, List<DbProjectUser>>(x => x.Map(It.IsAny<Guid>(), It.IsAny<List<UserRequest>>()), dbProjectUserMapperTimes);
      _mocker.Verify<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()), projectRepositoryTimes);
      _mocker.Verify<IProjectUserRepository>(x => x.EditIsActiveAsync(It.IsAny<List<DbProjectUser>>(), It.IsAny<Guid>()), projectUserRepositoryEditTimes);
      _mocker.Verify<IProjectUserRepository>(x => x.CreateAsync(It.IsAny<List<DbProjectUser>>()), projectUserRepositoryCreateTimes);
      _mocker.Verify<IGlobalCacheRepository>(x => x.RemoveAsync(It.IsAny<Guid>()), globalCacheRepositoryTimes);
      _mocker.Verify<IPublish>(x => x.CreateWorkTimeAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()), publishTimes);

      _mocker.Resolvers.Clear();
    }

    #region Setup

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _authorId = Guid.NewGuid();
      _userId = Guid.NewGuid();
      _projectId = Guid.NewGuid();

      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<CreateProjectUsersCommand>();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _authorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _userRequest = new UserRequest
      {
        UserId = _userId,
        Role = ProjectUserRoleType.Manager
      };

      _request = new CreateProjectUsersRequest
      {
        ProjectId = _projectId,
        Users = new List<UserRequest>
          {
            _userRequest
          }
      };

      _existingUsers = new List<DbProjectUser>();

      _mocker
        .Setup<IHttpContextAccessor, int>(a => a.HttpContext.Response.StatusCode)
        .Returns(200);
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<ICreateProjectUsersRequestValidator>().Reset();
      _mocker.GetMock<IProjectUserRepository>().Reset();
      _mocker.GetMock<IProjectRepository>().Reset();
      _mocker.GetMock<IDbProjectUserMapper>().Reset();
      _mocker.GetMock<IGlobalCacheRepository>().Reset();
      _mocker.GetMock<IPublish>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(true);

      _mocker
        .Setup<ICreateProjectUsersRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<CreateProjectUsersRequest>(), default).Result.IsValid)
        .Returns(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Request is not correct." }
        });

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Not enough rights." }
        });
    }

    #endregion

    [Test]
    public async Task UserHasNotRights()
    {
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(false);

      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_authorId, _request.ProjectId, true))
        .ReturnsAsync(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Not enough rights." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        сreateProjectUsersRequestValidatorTimes: Times.Never(),
        projectUserRepositoryGetTimes: Times.Never(),
        projectUserRepositoryCreateTimes: Times.Never(),
        projectUserRepositoryEditTimes: Times.Never(),
        projectRepositoryTimes: Times.Never(),
        dbProjectUserMapperTimes: Times.Never(),
        globalCacheRepositoryTimes: Times.Never(),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task RequestIsNotCorrect()
    {
      _mocker
        .Setup<ICreateProjectUsersRequestValidator, bool>(x => x.ValidateAsync(_request, default).Result.IsValid)
        .Returns(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Request is not correct." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        сreateProjectUsersRequestValidatorTimes: Times.Once(),
        projectUserRepositoryGetTimes: Times.Never(),
        projectUserRepositoryCreateTimes: Times.Never(),
        projectUserRepositoryEditTimes: Times.Never(),
        projectRepositoryTimes: Times.Never(),
        dbProjectUserMapperTimes: Times.Never(),
        globalCacheRepositoryTimes: Times.Never(),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task ProjectStatusIsNotActive()
    {
      _mocker
        .Setup<IProjectUserRepository, Task<List<DbProjectUser>>>(x => x.GetExistingUsersAsync(_request.ProjectId, It.IsAny<IEnumerable<Guid>>()))
        .ReturnsAsync(_existingUsers);

      _mocker
        .Setup<IDbProjectUserMapper, List<DbProjectUser>>(x => x.Map(_request.ProjectId, It.IsAny<List<UserRequest>>()))
        .Returns(_existingUsers);

      _mocker
        .Setup<IProjectUserRepository>(x => x.CreateAsync(It.IsAny<List<DbProjectUser>>()));

      _mocker
        .Setup<IProjectUserRepository>(x => x.EditIsActiveAsync(It.IsAny<List<DbProjectUser>>(), It.IsAny<Guid>()));

      DbProject dbProject = new DbProject
      {
        Id = Guid.NewGuid(),
        Status = (int)ProjectStatusType.Suspend
      };

      _mocker
        .Setup<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()))
        .ReturnsAsync(dbProject);

      _mocker
        .Setup<IGlobalCacheRepository>(x => x.RemoveAsync(_request.ProjectId));

      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = true,
        Errors = new List<string> { }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        сreateProjectUsersRequestValidatorTimes: Times.Once(),
        projectUserRepositoryGetTimes: Times.Once(),
        projectUserRepositoryCreateTimes: Times.Once(),
        projectUserRepositoryEditTimes: Times.Once(),
        projectRepositoryTimes: Times.Once(),
        dbProjectUserMapperTimes: Times.Once(),
        globalCacheRepositoryTimes: Times.Exactly(2),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task ProjectStatusIsActive()
    {
      _mocker
        .Setup<IProjectUserRepository, Task<List<DbProjectUser>>>(x => x.GetExistingUsersAsync(_request.ProjectId, It.IsAny<IEnumerable<Guid>>()))
        .ReturnsAsync(_existingUsers);

      _mocker
        .Setup<IDbProjectUserMapper, List<DbProjectUser>>(x => x.Map(_request.ProjectId, It.IsAny<List<UserRequest>>()))
        .Returns(_existingUsers);

      _mocker
        .Setup<IProjectUserRepository>(x => x.CreateAsync(It.IsAny<List<DbProjectUser>>()));

      _mocker
        .Setup<IProjectUserRepository>(x => x.EditIsActiveAsync(It.IsAny<List<DbProjectUser>>(), It.IsAny<Guid>()));

      DbProject dbProject = new DbProject
      {
        Id = Guid.NewGuid(),
        Status = (int)ProjectStatusType.Active
      };

      _mocker
        .Setup<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()))
        .ReturnsAsync(dbProject);

      _mocker
        .Setup<IPublish>(x => x.CreateWorkTimeAsync(_request.ProjectId, It.IsAny<List<Guid>>()));

      _mocker
        .Setup<IGlobalCacheRepository>(x => x.RemoveAsync(_request.ProjectId));

      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = true,
        Errors = new List<string> { }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        сreateProjectUsersRequestValidatorTimes: Times.Once(),
        projectUserRepositoryGetTimes: Times.Once(),
        projectUserRepositoryCreateTimes: Times.Once(),
        projectUserRepositoryEditTimes: Times.Once(),
        projectRepositoryTimes: Times.Once(),
        dbProjectUserMapperTimes: Times.Once(),
        globalCacheRepositoryTimes: Times.Exactly(2),
        publishTimes: Times.Once());
    }
  }
}
