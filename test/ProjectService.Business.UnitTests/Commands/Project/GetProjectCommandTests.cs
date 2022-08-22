using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
{
  internal class GetProjectCommandTests
  {
    private IGetProjectCommand _command;
    private AutoMocker _mocker;
    private List<ProjectUserInfo> _projectUsersInfo;
    private DbProject _dbProject;
    private DepartmentInfo _departmentInfo;
    private ProjectInfo _projectInfo;
    private ProjectResponse _response;

    private void Verifiable(
      Times responseCreatorTimes,
      Times projectRepositoryTimes,
      Times departmentServiceTimes,
      Times projectResponseMapperTimes,
      Times departmentInfoMapperTimes)
    {
      _mocker.Verify<IResponseCreator, OperationResultResponse<ProjectResponse>>(
        x => x.CreateFailureResponse<ProjectResponse>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IProjectRepository,Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()), projectRepositoryTimes);
      _mocker.Verify<IDepartmentService, Task<List<DepartmentData>>>(x => x.GetDepartmentsAsync(It.IsAny<List<string>>(), default, default), departmentServiceTimes);
      _mocker.Verify<IProjectResponseMapper, ProjectResponse > (x => x.Map(_dbProject, It.IsAny<DepartmentInfo>()), projectResponseMapperTimes);
      _mocker.Verify<IDepartmentInfoMapper, DepartmentInfo>(x => x.Map(It.IsAny<DepartmentData>()), departmentInfoMapperTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();

      _command = _mocker.CreateInstance<GetProjectCommand>();

      _dbProject = new DbProject
      {
        Id = Guid.NewGuid(),
        Name = "Project",
        Department = new()
      };

      _projectInfo = new ProjectInfo
      {
        Id = _dbProject.Id,
        Name = _dbProject.Name,
        Department = _departmentInfo
      };

      _projectUsersInfo = new List<ProjectUserInfo>
        {
          new ProjectUserInfo
          {
              UserId = Guid.NewGuid(),
              Role = ProjectUserRoleType.Manager
          }
        };

      _response = new ProjectResponse
      {
        Users = _projectUsersInfo
      };
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IProjectRepository>().Reset();
      _mocker.GetMock<IDepartmentService>().Reset();
      _mocker.GetMock<IProjectResponseMapper>().Reset();
      _mocker.GetMock<IDepartmentInfoMapper>().Reset();
    }

    [Test]
    public async Task NotFoundProject()
    {
      _mocker
        .Setup<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()))
        .ReturnsAsync(It.IsAny<DbProject>());

      OperationResultResponse<ProjectResponse> expectedResponse = new()
      {
        Errors = new List<string> { "Not found." }
      };

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<ProjectResponse>>(x => x.CreateFailureResponse<ProjectResponse>(HttpStatusCode.NotFound, default))
        .Returns(expectedResponse);


      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(It.IsAny<GetProjectFilter>()));
    }

    [Test]
    public async Task ProjectDepartmentIsNotNull()
    {
      _mocker
        .Setup<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()))
        .ReturnsAsync(_dbProject);

      _mocker
        .Setup<IDepartmentService, Task<List<DepartmentData>>>(x => x.GetDepartmentsAsync(It.IsAny<List<string>>(), default, default))
        .ReturnsAsync(It.IsAny<List<DepartmentData>>());

      _mocker
        .Setup<IProjectResponseMapper, ProjectResponse>(x => x.Map(_dbProject, It.IsAny<DepartmentInfo>()))
        .Returns(_response);

      _mocker
        .Setup<IDepartmentInfoMapper, DepartmentInfo>(x => x.Map(It.IsAny<DepartmentData>()))
        .Returns(It.IsAny<DepartmentInfo>());

      OperationResultResponse<ProjectResponse> expectedResponse = new()
      {
        Body = _response
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(It.IsAny<GetProjectFilter>()));
    }

    [Test]
    public async Task ProjectDepartmentIsNull()
    {
      DbProject _dbProject = new DbProject
      {
        Id = Guid.NewGuid(),
        Name = "Project"
      };

      _mocker
        .Setup<IProjectRepository, Task<DbProject>>(x => x.GetAsync(It.IsAny<GetProjectFilter>()))
        .ReturnsAsync(_dbProject);

      _mocker
        .Setup<IProjectResponseMapper, ProjectResponse>(x => x.Map(_dbProject, It.IsAny<DepartmentInfo>()))
        .Returns(_response);

      _mocker
        .Setup<IDepartmentInfoMapper, DepartmentInfo>(x => x.Map(It.IsAny<DepartmentData>()))
        .Returns(It.IsAny<DepartmentInfo>());

      OperationResultResponse<ProjectResponse> expectedResponse = new()
      {
        Body = _response
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(It.IsAny<GetProjectFilter>()));
    }
  }
}
