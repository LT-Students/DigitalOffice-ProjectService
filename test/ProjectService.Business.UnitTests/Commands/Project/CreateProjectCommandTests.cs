using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
{
  internal class CreateProjectCommandTests
  {
    private Guid _authorId;
    private AutoMocker _mocker;
    private DbProject _dbProject;
    private CreateProjectRequest _request;
    private ICreateProjectCommand _command;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times createProjectRequestValidatorTimes,
      Times imageServiceTimes,
      Times dbProjectMapperTimes,
      Times projectRepositoryTimes,
      Times publishTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<ICreateProjectRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<CreateProjectRequest>(), default).Result.IsValid, createProjectRequestValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<Guid?>>(
        x => x.CreateFailureResponse<Guid?>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IImageService, Task<List<Guid>>>(x => x.CreateImagesAsync(It.IsAny<List<ImageContent>>(), It.IsAny<List<string>>()), imageServiceTimes);
      _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(It.IsAny<CreateProjectRequest>(), It.IsAny<List<Guid>>()), dbProjectMapperTimes);
      _mocker.Verify<IProjectRepository>(x => x.CreateAsync(It.IsAny<DbProject>()), projectRepositoryTimes);
      _mocker.Verify<IPublish>(x => x.CreateWorkTimeAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()), publishTimes);

      _mocker.Resolvers.Clear();
    }

    #region Setup

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _authorId = Guid.NewGuid();

      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<CreateProjectCommand>();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _authorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _request = new CreateProjectRequest
      {
        Name = "Project for Lanit-Tercom",
        ShortName = "Project",
        Description = "New project for Lanit-Tercom",
        ShortDescription = "Short description",
        Customer = "Customer",
        Status = ProjectStatusType.Active,
        Users = new List<UserRequest>
          {
            new UserRequest
            {
                UserId = Guid.NewGuid(),
                Role = ProjectUserRoleType.Manager
            }
          }
      };

      _dbProject = new DbProject
      {
        Id = Guid.NewGuid(),
        Status = (int)_request.Status,
        ShortName = _request.ShortName,
        Name = _request.Name,
        Description = _request.Description,
        ShortDescription = _request.ShortDescription,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = Guid.NewGuid(),
        Users = new List<DbProjectUser>
          {
            new DbProjectUser
            {
                Id = Guid.NewGuid(),
                Role = (int)_request.Users.FirstOrDefault().Role,
                IsActive = true
            }
          }
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
      _mocker.GetMock<ICreateProjectRequestValidator>().Reset();
      _mocker.GetMock<IImageService>().Reset();
      _mocker.GetMock<IDbProjectMapper>().Reset();
      _mocker.GetMock<IProjectRepository>().Reset();
      _mocker.GetMock<IPublish>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(true);

      _mocker
        .Setup<ICreateProjectRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<CreateProjectRequest>(), default).Result.IsValid)
        .Returns(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<Guid?>>(x => x.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<Guid?>()
        {
          Errors = new() { "Request is not correct." }
        });

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<Guid?>>(x => x.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<Guid?>()
        {
          Errors = new() { "Not enough rights." }
        });
    }

    #endregion

    [Test]
    public async Task ShouldThrowExceptionWhenUserHasNotRights()
    {
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(false);

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string> { "Not enough rights." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        createProjectRequestValidatorTimes: Times.Never(),
        imageServiceTimes: Times.Never(),
        dbProjectMapperTimes: Times.Never(),
        projectRepositoryTimes: Times.Never(),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task ShouldThrowExceptionWhenCreatingNewProjectWithIncorrectProjectData()
    {
      _mocker
        .Setup<ICreateProjectRequestValidator, bool>(x => x.ValidateAsync(_request, default).Result.IsValid)
        .Returns(false);

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string> { "Request is not correct." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        createProjectRequestValidatorTimes: Times.Once(),
        imageServiceTimes: Times.Never(),
        dbProjectMapperTimes: Times.Never(),
        projectRepositoryTimes: Times.Never(),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task ResultTestWithCreationWorkTime()
    {
      _mocker
        .Setup<IImageService, Task<List<Guid>>>(x => x.CreateImagesAsync(It.IsAny<List<ImageContent>>(), default))
        .ReturnsAsync(It.IsAny<List<Guid>>());

      _mocker
        .Setup<IDbProjectMapper, DbProject>(x => x.Map(_request, It.IsAny<List<Guid>>()))
        .Returns(_dbProject);

      _mocker
        .Setup<IProjectRepository>(x => x.CreateAsync(_dbProject));

      _mocker
        .Setup<IPublish>(x => x.CreateWorkTimeAsync(_dbProject.Id, It.IsAny<List<Guid>>()));

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Body = _dbProject.Id,
        Errors = new List<string> {}
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        createProjectRequestValidatorTimes: Times.Once(),
        imageServiceTimes: Times.Once(),
        dbProjectMapperTimes: Times.Once(),
        projectRepositoryTimes: Times.Once(),
        publishTimes: Times.Once());
    }

    [Test]
    public async Task ResultTestWithoutCreationWorkTime()
    {
      CreateProjectRequest _request = new CreateProjectRequest
      {
        Name = "Project for Lanit-Tercom",
        ShortName = "Project",
        Description = "New project for Lanit-Tercom",
        ShortDescription = "Short description",
        Customer = "Customer",
        Status = ProjectStatusType.Active,
        Users = new List<UserRequest>()
      };

      _mocker
        .Setup<IImageService, Task<List<Guid>>>(x => x.CreateImagesAsync(It.IsAny<List<ImageContent>>(), default))
        .ReturnsAsync(It.IsAny<List<Guid>>());

      _mocker
        .Setup<IDbProjectMapper, DbProject>(x => x.Map(_request, It.IsAny<List<Guid>>()))
        .Returns(_dbProject);

      _mocker
        .Setup<IProjectRepository>(x => x.CreateAsync(_dbProject));

      _mocker
        .Setup<IPublish>(x => x.CreateWorkTimeAsync(_dbProject.Id, It.IsAny<List<Guid>>()));

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Body = _dbProject.Id,
        Errors = new List<string> { }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        createProjectRequestValidatorTimes: Times.Once(),
        imageServiceTimes: Times.Once(),
        dbProjectMapperTimes: Times.Once(),
        projectRepositoryTimes: Times.Once(),
        publishTimes: Times.Never());
    }
  }
}
