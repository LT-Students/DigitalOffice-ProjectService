using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using LT.DigitalOffice.ProjectService.Validation.User.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands.ProjectUsers
{
  internal class EditProjectUsersRoleCommandTests
  {
    private AutoMocker _mocker;
    private EditProjectUsersRoleRequest _request;
    private Guid _projectId;
    private Guid _authorId;
    private List<Guid> _usersIds;

    private IEditProjectUsersRoleCommand _command;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times editProjectUsersRoleRequestValidatorTimes,
      Times projectUserRepositoryTimesExists,
      Times projectUserRepositoryTimes,
      Times globalCacheRepositoryTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IEditProjectUsersRoleRequestValidator, bool>(
        x => x.ValidateAsync(It.IsAny<(Guid projectId, EditProjectUsersRoleRequest request)>(), default).Result.IsValid, editProjectUsersRoleRequestValidatorTimes);
      _mocker.Verify<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_projectId, _authorId, true), projectUserRepositoryTimesExists);
      _mocker.Verify<IProjectUserRepository, Task<bool>>(x => x.EditAsync(_projectId, _request), projectUserRepositoryTimes);
      _mocker.Verify<IGlobalCacheRepository>(x => x.RemoveAsync(_projectId), globalCacheRepositoryTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<EditProjectUsersRoleCommand>();

      _projectId = Guid.NewGuid();
      _authorId = Guid.NewGuid();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _authorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _usersIds = new List<Guid>
      {
        Guid.NewGuid(),
        Guid.NewGuid()
      };

      _request = new EditProjectUsersRoleRequest
      {
        Role = ProjectUserRoleType.Employee,
        UsersIds = _usersIds
      };

      _mocker
        .Setup<IHttpContextAccessor, int>(a => a.HttpContext.Response.StatusCode)
        .Returns(200);
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IEditProjectUsersRoleRequestValidator>().Reset();
      _mocker.GetMock<IProjectUserRepository>().Reset();
      _mocker.GetMock<IGlobalCacheRepository>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(true);

      _mocker
        .Setup<IEditProjectUsersRoleRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<(Guid projectId, EditProjectUsersRoleRequest request)>(), default).Result.IsValid)
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

    [Test]
    public async Task UserDoesNotHaveRights()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Not enough rights." }
      };

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(false);

      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_projectId, _authorId, true))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        editProjectUsersRoleRequestValidatorTimes: Times.Never(),
        projectUserRepositoryTimesExists: Times.Once(),
        projectUserRepositoryTimes: Times.Never(),
        globalCacheRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task RequestIsNotCorrect()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Request is not correct." }
      };

      _mocker
        .Setup<IEditProjectUsersRoleRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<(Guid projectId, EditProjectUsersRoleRequest request)>(), default).Result.IsValid)
        .Returns(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        editProjectUsersRoleRequestValidatorTimes: Times.Once(),
        projectUserRepositoryTimesExists: Times.Once(),
        projectUserRepositoryTimes: Times.Never(),
        globalCacheRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task NotSuccessResult()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = false
      };

      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.EditAsync(_projectId, _request))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        editProjectUsersRoleRequestValidatorTimes: Times.Once(),
        projectUserRepositoryTimesExists: Times.Once(),
        projectUserRepositoryTimes: Times.Once(),
        globalCacheRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task SuccessResult()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = true
      };

      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.EditAsync(_projectId, _request))
        .ReturnsAsync(true);

      _mocker
        .Setup<IGlobalCacheRepository>(x => x.RemoveAsync(_projectId));

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        editProjectUsersRoleRequestValidatorTimes: Times.Once(),
        projectUserRepositoryTimesExists: Times.Once(),
        projectUserRepositoryTimes: Times.Once(),
        globalCacheRepositoryTimes: Times.Once());
    }
  }
}
