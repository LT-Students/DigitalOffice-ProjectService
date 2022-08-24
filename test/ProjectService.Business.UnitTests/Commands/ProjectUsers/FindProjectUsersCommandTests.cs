using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
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

/*    private List<(DbProject dbProject, int usersCount)> _dbProjects;
    private DbProject _dbProject;*/
    private ProjectInfo _projectInfo;
    private FindProjectUsersFilter _filter;

    private Guid _projectId;
    private const string Name = "Name";
    private const string ShortName = "ShortName";

/*    private void Verifiable(
      Times baseFindFilterValidatorTimes,
      Times responseCreatorTimes,
      Times projectRepositoryTimes,
      Times projectInfoMapperTimes,
      Times departmentServiceTimess)
    {
      _mocker.Verify<IBaseFindFilterValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, baseFindFilterValidatorTimes);
      _mocker.Verify<IResponseCreator, FindResultResponse<ProjectInfo>>(
        x => x.CreateFailureFindResponse<ProjectInfo>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IProjectRepository, Task<(List<(DbProject dbProject, int usersCount)> dbProjects, int totalCount)>>(
        x => x.FindAsync(It.IsAny<FindProjectsFilter>()), projectRepositoryTimes);
      _mocker.Verify<IProjectInfoMapper, ProjectInfo>(x => x.Map(_dbProject, 0, It.IsAny<DepartmentInfo>()), projectInfoMapperTimes);
      _mocker.Verify<IDepartmentService, Task<List<DepartmentData>>>(x => x.GetDepartmentsAsync(It.IsAny<List<string>>(), It.IsAny<List<Guid>>(), default), departmentServiceTimess);

      _mocker.Resolvers.Clear();
    }
*/
    #region Setup

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<FindProjectUsersCommand>();

      _filter = new FindProjectUsersFilter
      {
        IsActive = true
      };

      _projectId = Guid.NewGuid();

/*      _dbProject = new DbProject()
      {
        Id = Guid.NewGuid(),
        Name = Name,
        ShortName = ShortName
      };

      _dbProjects = new List<(DbProject dbProject, int usersCount)>
      {
        (_dbProject, 0)
      };*/
/*
      _projectInfo = new ProjectInfo
      {
        Id = _dbProject.Id,
        Name = _dbProject.Name,
        ShortName = _dbProject.ShortName
      };*/
    }

    [SetUp]
    public void SetUp()
    {
/*      _mocker.GetMock<IBaseFindFilterValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IProjectRepository>().Reset();
      _mocker.GetMock<IProjectInfoMapper>().Reset();
      _mocker.GetMock<IDepartmentService>().Reset();*/

      _mocker
        .Setup<IBaseFindFilterValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
        .Returns(true);
    }

    #endregion

    [Test]
    public async Task FilterNotCorrect()
    {
      FindProjectUsersFilter _findProjectsFilter = new FindProjectUsersFilter
      {
        SkipCount = -1,
        TakeCount = 100
      };

      _mocker
        .Setup<IBaseFindFilterValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
        .Returns(false);

      FindResultResponse<UserInfo> expectedResponse = new FindResultResponse<UserInfo>
      {
        TotalCount = 0
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_projectId, _findProjectsFilter));

/*      Verifiable(
        baseFindFilterValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        projectRepositoryTimes: Times.Never(),
        projectInfoMapperTimes: Times.Never(),
        departmentServiceTimess: Times.Never());*/
    }

    [Test]
    public async Task IncludeDepartmentIsFalse()
    {
/*      FindProjectsFilter _findProjectsFilter = new FindProjectsFilter
      {
        SkipCount = 0,
        TakeCount = 100
      };

      _mocker
        .Setup<IProjectRepository, Task<(List<(DbProject dbProject, int usersCount)> dbProjects, int totalCount)>>(x => x.FindAsync(_findProjectsFilter))
        .ReturnsAsync((_dbProjects, 1));

      _mocker
        .Setup<IProjectInfoMapper, ProjectInfo>(x => x.Map(_dbProject, 0, It.IsAny<DepartmentInfo>()))
        .Returns(_projectInfo);

      FindResultResponse<ProjectInfo> expectedResponse = new()
      {
        TotalCount = 1,
        Body = new List<ProjectInfo>() {
          _projectInfo
        }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_findProjectsFilter));*/

/*      Verifiable(
        baseFindFilterValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        projectRepositoryTimes: Times.Once(),
        projectInfoMapperTimes: Times.Once(),
        departmentServiceTimess: Times.Never());*/
    }

    [Test]
    public async Task IncludeDepartmentIsTrue()
    {
/*      FindProjectsFilter _findProjectsFilter = new FindProjectsFilter
      {
        SkipCount = 0,
        TakeCount = 100,
        IncludeDepartment = true
      };

      _mocker
        .Setup<IProjectRepository, Task<(List<(DbProject dbProject, int usersCount)> dbProjects, int totalCount)>>(x => x.FindAsync(_findProjectsFilter))
        .ReturnsAsync((_dbProjects, 1));

      _mocker
        .Setup<IDepartmentService, Task<List<DepartmentData>>>(x => x.GetDepartmentsAsync(It.IsAny<List<string>>(), It.IsAny<List<Guid>>(), default))
        .ReturnsAsync(It.IsAny<List<DepartmentData>>());

      _mocker
        .Setup<IProjectInfoMapper, ProjectInfo>(x => x.Map(_dbProject, 0, It.IsAny<DepartmentInfo>()))
        .Returns(_projectInfo);

      FindResultResponse<ProjectInfo> expectedResponse = new()
      {
        TotalCount = 1,
        Body = new List<ProjectInfo>() {
          _projectInfo
        }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_findProjectsFilter));*/

/*      Verifiable(
        baseFindFilterValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        projectRepositoryTimes: Times.Once(),
        projectInfoMapperTimes: Times.Once(),
        departmentServiceTimess: Times.Once());*/
    }
  }
}
