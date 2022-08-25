using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
{
  class RemoveProjectUsersCommandTests
  {
    private IRemoveProjectUsersCommand _command;
    private AutoMocker _mocker;

    private Guid _projectId;
    private Guid _authorId;
    private List<Guid> _userIds;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times projectUserRepositoryExistsTimes,
      Times projectUserRepositoryRemoveTimes,
      Times globalCacheRepositoryTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_authorId, _projectId, true), projectUserRepositoryExistsTimes);
      _mocker.Verify<IProjectUserRepository, Task<bool>>(x => x.RemoveAsync(_projectId, _userIds), projectUserRepositoryRemoveTimes);
      _mocker.Verify<IGlobalCacheRepository>(x => x.RemoveAsync(It.IsAny<Guid>()), globalCacheRepositoryTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<RemoveProjectUsersCommand>();

      _authorId = Guid.NewGuid();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _authorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _projectId = Guid.NewGuid();
      _userIds = new List<Guid>
      {
        Guid.NewGuid(),
        Guid.NewGuid()
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
      _mocker.GetMock<IProjectUserRepository>().Reset();
      _mocker.GetMock<IGlobalCacheRepository>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Request is not correct." }
        });

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool> ()
        {
          Errors = new() { "Not enough rights." }
        });
    }

    [Test]
    public async Task UserHasNotRights()
    {
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(false);

      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_authorId, _projectId, true))
        .ReturnsAsync(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Not enough rights." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _userIds));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        projectUserRepositoryExistsTimes: Times.Once(),
        projectUserRepositoryRemoveTimes: Times.Never(),
        globalCacheRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task RequestIsNotCorrect()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Request is not correct." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, null));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        projectUserRepositoryExistsTimes: Times.Never(),
        projectUserRepositoryRemoveTimes: Times.Never(),
        globalCacheRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task NotSuccessResult()
    {
      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.RemoveAsync(_projectId, _userIds))
        .ReturnsAsync(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = false
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _userIds));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        projectUserRepositoryExistsTimes: Times.Never(),
        projectUserRepositoryRemoveTimes: Times.Once(),
        globalCacheRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task SuccessResult()
    {
      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.RemoveAsync(_projectId, _userIds))
        .ReturnsAsync(true);

      _mocker
        .Setup<IGlobalCacheRepository>(x => x.RemoveAsync(_projectId));

      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = true
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _userIds));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        projectUserRepositoryExistsTimes: Times.Never(),
        projectUserRepositoryRemoveTimes: Times.Once(),
        globalCacheRepositoryTimes: Times.Once());
    }
  }
}
