using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Business.Commands.Project;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands.UnitTests
{
  internal class EditProjectCommandTests
  {
    private AutoMocker _mocker;
    private JsonPatchDocument<EditProjectRequest> _request;
    private DbProject _dbProject;
    private IDictionary<object, object> _items;
    private Guid _projectId;

    private IEditProjectCommand _command;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times editProjectRequestValidatorTimes,
      Times projectUserRepositoryExistsTimes,
      Times projectRepositoryGetTimes,
      Times patchDbProjectMapperTimes,
      Times projectRepositoryEditTimes,
      Times globalCacheRepositoryTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<IEditProjectRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<JsonPatchDocument<EditProjectRequest>>(), default).Result.IsValid, editProjectRequestValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), true), projectUserRepositoryExistsTimes);
      _mocker.Verify<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()), projectRepositoryGetTimes);
      _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x =>
            x.Map(It.IsAny<JsonPatchDocument<EditProjectRequest>>()), patchDbProjectMapperTimes);
      _mocker.Verify<IProjectRepository, Task<bool>>(x => x.EditAsync(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbProject>>()), projectRepositoryEditTimes);
      _mocker.Verify<IGlobalCacheRepository>(x => x.RemoveAsync(It.IsAny<Guid>()), globalCacheRepositoryTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<EditProjectCommand>();

      _request = new JsonPatchDocument<EditProjectRequest>(
        new List<Operation<EditProjectRequest>>
        {
          new Operation<EditProjectRequest>(
            "replace",
            $"/{nameof(EditProjectRequest.Name)}",
            "",
            "Name"),
          new Operation<EditProjectRequest>(
            "replace",
            $"/{nameof(EditProjectRequest.Description)}",
            "",
            "Description"),
          new Operation<EditProjectRequest>(
            "replace",
            $"/{nameof(EditProjectRequest.Status)}",
            "",
            "Active")
        }, new CamelCasePropertyNamesContractResolver());

      _projectId = Guid.NewGuid();
      _dbProject = new DbProject {
        Id = _projectId,
        Status = (int)ProjectStatusType.Suspend,
        Users = new List<DbProjectUser>()
      };

      _items = new Dictionary<object, object>();
      _items.Add("UserId", Guid.NewGuid());

      _mocker
        .Setup<IHttpContextAccessor, int>(a => a.HttpContext.Response.StatusCode)
        .Returns(400);
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IEditProjectRequestValidator>().Reset();
      _mocker.GetMock<IProjectRepository>().Reset();
      _mocker.GetMock<IPatchDbProjectMapper>().Reset();
      _mocker.GetMock<IGlobalCacheRepository>().Reset();

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(true);

      _mocker
        .Setup<IEditProjectRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<JsonPatchDocument<EditProjectRequest>>(), default).Result.IsValid)
        .Returns(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Request is not correct." }
        });

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.Forbidden, default))
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
        .Setup<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync((Guid)_items["UserId"], _projectId, true))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        editProjectRequestValidatorTimes: Times.Never(),
        projectUserRepositoryExistsTimes: Times.Once(),
        projectRepositoryGetTimes: Times.Never(),
        patchDbProjectMapperTimes: Times.Never(),
        projectRepositoryEditTimes: Times.Never(),
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
        .Setup<IEditProjectRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<JsonPatchDocument<EditProjectRequest>>(), default).Result.IsValid)
        .Returns(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        editProjectRequestValidatorTimes: Times.Once(),
        projectUserRepositoryExistsTimes: Times.Never(),
        projectRepositoryGetTimes: Times.Never(),
        patchDbProjectMapperTimes: Times.Never(),
        projectRepositoryEditTimes: Times.Never(),
        globalCacheRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task ResponseBodyIsFalse()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = false
      };

      _mocker
        .Setup<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()))
        .ReturnsAsync(_dbProject);

      _mocker
        .Setup<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x =>
            x.Map(It.IsAny<JsonPatchDocument<EditProjectRequest>>()))
        .Returns(It.IsAny<JsonPatchDocument<DbProject>>());

      _mocker
        .Setup<IProjectRepository, Task<bool>>(x => x.EditAsync(_projectId, It.IsAny<JsonPatchDocument<DbProject>>()))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        editProjectRequestValidatorTimes: Times.Once(),
        projectUserRepositoryExistsTimes: Times.Never(),
        projectRepositoryGetTimes: Times.Once(),
        patchDbProjectMapperTimes: Times.Once(),
        projectRepositoryEditTimes: Times.Once(),
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
        .Setup<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()))
        .ReturnsAsync(_dbProject);

      _mocker
        .Setup<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x =>
          x.Map(It.IsAny<JsonPatchDocument<EditProjectRequest>>()))
        .Returns(It.IsAny<JsonPatchDocument<DbProject>>());

      _mocker
        .Setup<IProjectRepository, Task<bool>>(x => x.EditAsync(_projectId, It.IsAny<JsonPatchDocument<DbProject>>()))
        .ReturnsAsync(true);

      _mocker
        .Setup<IGlobalCacheRepository>(x => x.RemoveAsync(_projectId));

      _mocker
        .Setup<IProjectRepository, Task<bool>>(x => x.EditAsync(_projectId, It.IsAny<JsonPatchDocument<DbProject>>()))
        .ReturnsAsync(true);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        editProjectRequestValidatorTimes: Times.Once(),
        projectUserRepositoryExistsTimes: Times.Never(),
        projectRepositoryGetTimes: Times.Exactly(2),
        patchDbProjectMapperTimes: Times.Once(),
        projectRepositoryEditTimes: Times.Once(),
        globalCacheRepositoryTimes: Times.Once());
    }
  }
}
