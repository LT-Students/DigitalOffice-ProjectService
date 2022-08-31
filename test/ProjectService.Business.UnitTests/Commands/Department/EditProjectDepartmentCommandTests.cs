using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Department;
using LT.DigitalOffice.ProjectService.Business.Commands.Department.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Department;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands.Department
{
  internal class EditProjectDepartmentCommandTests
  {
    private readonly Guid _createdBy = Guid.NewGuid();

    private AutoMocker _mocker;
    private EditProjectDepartmentRequest _request;
    private EditProjectDepartmentRequest _requestWithNullDepartmentId;
    private Guid _projectId;
    private Guid _departmentId;
    private Dictionary<object, object> _items;
    private DbProjectDepartment _dbProjectDepartment;

    private IEditProjectDepartmentCommand _command;

    private void Verifiable(
      Times accessValidatorTimes,
      Times httpContextAccessorTimes,
      Times responseCreatorTimes,
      Times projectDepartmentRepositoryEditTimes,
      Times projectDepartmentRepositoryCreateTimes,
      Times projectDepartmentRepositoryGetTimes,
      Times dbProjectDepartmentMapperTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x =>
          x.HasRightsAsync(It.IsAny<int>()),
        accessValidatorTimes);

      _mocker.Verify<IHttpContextAccessor, IDictionary <object, object>>(x =>
          x.HttpContext.Items,
        httpContextAccessorTimes);

      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(x =>
          x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorTimes);

      _mocker.Verify<IProjectDepartmentRepository, Task<bool>>(x =>
          x.EditAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()),
        projectDepartmentRepositoryEditTimes);
      _mocker.Verify<IProjectDepartmentRepository>(x =>
          x.CreateAsync(It.IsAny<DbProjectDepartment>()),
        projectDepartmentRepositoryCreateTimes);
      _mocker.Verify<IProjectDepartmentRepository, Task<DbProjectDepartment>>(x =>
          x.GetAsync(_request.ProjectId),
        projectDepartmentRepositoryGetTimes);

      _mocker.Verify<IDbProjectDepartmentMapper, DbProjectDepartment>(x =>
          x.Map(It.IsAny<Guid>(), It.IsAny<Guid>()),
        dbProjectDepartmentMapperTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<EditProjectDepartmentCommand>();

      _projectId = Guid.NewGuid();
      _departmentId = Guid.NewGuid();

      _request = new EditProjectDepartmentRequest()
      {
        ProjectId = _projectId,
        DepartmentId = _departmentId
      };

      _requestWithNullDepartmentId = new EditProjectDepartmentRequest()
      {
        ProjectId = _projectId
      };

      _dbProjectDepartment = new DbProjectDepartment { ProjectId = _request.ProjectId };

      _items = new() { { "UserId", _createdBy } };
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IHttpContextAccessor>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IProjectDepartmentRepository>().Reset();
      _mocker.GetMock<IDbProjectDepartmentMapper>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveDepartments))
        .ReturnsAsync(true);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object,object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _mocker
        .Setup<IProjectDepartmentRepository, Task<DbProjectDepartment>>(x => x.GetAsync(_request.ProjectId))
        .ReturnsAsync(_dbProjectDepartment);

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
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveDepartments))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        httpContextAccessorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        projectDepartmentRepositoryEditTimes: Times.Never(),
        projectDepartmentRepositoryCreateTimes: Times.Never(),
        projectDepartmentRepositoryGetTimes: Times.Once(),
        dbProjectDepartmentMapperTimes: Times.Never());
    }

    [Test]
    public async Task DepartmentEdit()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = true
      };

      _mocker
        .Setup<IProjectDepartmentRepository, Task<bool>>(x => x.EditAsync(_request.ProjectId, _request.DepartmentId))
        .ReturnsAsync(true);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        httpContextAccessorTimes: Times.Exactly(2),
        responseCreatorTimes: Times.Never(),
        projectDepartmentRepositoryEditTimes: Times.Once(),
        projectDepartmentRepositoryCreateTimes: Times.Never(),
        projectDepartmentRepositoryGetTimes: Times.Once(),
        dbProjectDepartmentMapperTimes: Times.Never());
    }

    [Test]
    public async Task DepartmentIsNotEditAndDepartmentIdIsNull()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Request is not correct." }
      };

      _mocker
        .Setup<IProjectDepartmentRepository, Task<bool>>(x => x.EditAsync(_requestWithNullDepartmentId.ProjectId, default))
        .ReturnsAsync(false);

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_requestWithNullDepartmentId));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        httpContextAccessorTimes: Times.Exactly(2),
        responseCreatorTimes: Times.Once(),
        projectDepartmentRepositoryEditTimes: Times.Once(),
        projectDepartmentRepositoryCreateTimes: Times.Never(),
        projectDepartmentRepositoryGetTimes: Times.Once(),
        dbProjectDepartmentMapperTimes: Times.Never());
    }

    [Test]
    public async Task DepartmentIsNotEditAndDepartmentIdIsNotNull()
    {
      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = true
      };

      _mocker
        .Setup<IProjectDepartmentRepository, Task<bool>>(x => x.EditAsync(_request.ProjectId, _request.DepartmentId))
        .ReturnsAsync(false);

      _mocker
        .Setup<IProjectDepartmentRepository>(x => x.CreateAsync(It.IsAny<DbProjectDepartment>()));

      _mocker
        .Setup<IDbProjectDepartmentMapper, DbProjectDepartment>(x => x.Map(_request.ProjectId, _request.DepartmentId.Value))
        .Returns(new DbProjectDepartment());

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        httpContextAccessorTimes: Times.Exactly(2),
        responseCreatorTimes: Times.Never(),
        projectDepartmentRepositoryEditTimes: Times.Once(),
        projectDepartmentRepositoryCreateTimes: Times.Once(),
        projectDepartmentRepositoryGetTimes: Times.Once(),
        dbProjectDepartmentMapperTimes: Times.Once());
    }
  }
}
