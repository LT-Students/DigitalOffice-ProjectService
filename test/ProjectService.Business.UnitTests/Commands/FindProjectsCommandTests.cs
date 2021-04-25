using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using NUnit.Framework;
using Moq.AutoMock;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using MassTransit;
using LT.DigitalOffice.ProjectService.Models.Db;
using System.Collections.Generic;
using Moq;
using LT.DigitalOffice.ProjectService.Models.Dto.Request.Filters;
using System;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using System.Threading.Tasks;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class FindProjectsCommandTests
    {
        private IFindProjectsCommand _command;
        private AutoMocker _mocker;
        private Mock<Response<IOperationResult<IFindDepartmentsResponse>>> _brokerFindResponseMock;

        private int _totalCount;
        private List<DbProject> _dbProjects;
        private Dictionary<Guid, string> _idNameFind;
        private Dictionary<Guid, string> _idNameGet;
        private DbProject _dbProject;
        private ProjectsResponse _projectsResponse;
        private FindProjectsFilter _findProjectsFilter;
        private FindDbProjectsFilter _findDbProjectsFilter;
        private ProjectInfo _projectInfo;

        private const string Name = "Name";
        private const string ShortName = "ShortName";
        private const string DepartmentName = "DepartmentName";
        private Guid DepartmentId;

        #region Setup

        public void SetUpModels()
        {
            DepartmentId = Guid.NewGuid();

            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<FindProjectsCommand>();

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = Name,
                ShortName = ShortName,
                ShortDescription = "description",
                DepartmentId = DepartmentId,
                CreatedAt = DateTime.UtcNow
            };

            _idNameFind = new();
            _idNameFind.Add(DepartmentId, DepartmentName);

            _dbProjects = new List<DbProject>
            {
                _dbProject
            };

            _projectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name,
                ShortName = _dbProject.ShortName,
                ShortDescription = _dbProject.ShortDescription,
                Department = new DepartmentInfo
                {
                    Id = DepartmentId,
                    Name = DepartmentName
                }
            };

            _projectsResponse = new ProjectsResponse
            {
                TotalCount = 1,
                Errors = null,
                Projects = new List<ProjectInfo>
                {
                    _projectInfo
                }
            };

            _idNameGet = new Dictionary<Guid, string>();
            _idNameGet.Add(_dbProject.DepartmentId, DepartmentName);

            _findProjectsFilter = new FindProjectsFilter
            {
                Name = Name
            };

            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                Name = _findProjectsFilter.Name
            };

            _brokerFindResponseMock = new Mock<Response<IOperationResult<IFindDepartmentsResponse>>>();
            _brokerFindResponseMock
                .Setup(x => x.Message.Body.IdNamePairs)
                .Returns(_idNameFind);
        }

        [SetUp]
        public void SetUp()
        {
            SetUpModels();
            _mocker.GetMock<IProjectRepository>().Reset();
            _mocker.GetMock<IFindDbProjectFilterMapper>().Reset();
            _mocker.GetMock<IFindProjectsResponseMapper>().Reset();

            _brokerFindResponseMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(true);
            _brokerFindResponseMock
                .Setup(x => x.Message.Errors)
                .Returns(null as List<string>);

            _mocker
                .Setup<IRequestClient<IFindDepartmentsRequest>, Task<Response<IOperationResult<IFindDepartmentsResponse>>>>(
                    x => x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_brokerFindResponseMock.Object));
        }

        #endregion

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFilterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _command.Execute(null, 0, 0));
            _mocker
                .Verify<IRequestClient<IFindDepartmentsRequest>>(x =>
                    x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        It.IsAny<object>(), default, default),
                    Times.Never);
            _mocker
                .Verify<IProjectRepository>(x =>
                    x.FindProjects(
                        It.IsAny<FindDbProjectsFilter>(), It.IsAny<int>(), It.IsAny<int>(), out _totalCount),
                    Times.Never);
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByName()
        {
            int skip = 0;
            int take = 2;

            _mocker
                .Setup<IFindDbProjectFilterMapper, FindDbProjectsFilter>(x => x.Map(
                    _findProjectsFilter,
                    null))
                .Returns(_findDbProjectsFilter);

            _mocker
                .Setup<IProjectRepository, List<DbProject>>(x => x.FindProjects(
                    _findDbProjectsFilter,
                    skip,
                    take,
                    out _totalCount))
                .Returns(_dbProjects);

            _mocker
                .Setup<IFindProjectsResponseMapper, ProjectsResponse>(x => x.Map(
                    _dbProjects,
                    It.IsAny<int>(),
                    _idNameGet,
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);

            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, skip, take));
            _mocker
                .Verify<IRequestClient<IFindDepartmentsRequest>>(x =>
                    x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        It.IsAny<object>(), default, default),
                    Times.Once);
            _mocker
                .Verify<IProjectRepository>(x =>
                    x.FindProjects(
                        _findDbProjectsFilter, skip, take, out _totalCount),
                    Times.Once);
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByShortName()
        {
            _findProjectsFilter = new FindProjectsFilter
            {
                ShortName = ShortName
            };
            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                ShortName = ShortName
            };

            int skip = 0;
            int take = 2;

            _mocker
                .Setup<IFindDbProjectFilterMapper, FindDbProjectsFilter>(x => x.Map(
                    _findProjectsFilter,
                    null))
                .Returns(_findDbProjectsFilter);

            _mocker
                .Setup<IProjectRepository, List<DbProject>>(x => x.FindProjects(
                    _findDbProjectsFilter,
                    skip,
                    take,
                    out _totalCount))
                .Returns(_dbProjects);

            _mocker
                .Setup<IFindProjectsResponseMapper, ProjectsResponse>(x => x.Map(
                    _dbProjects,
                    It.IsAny<int>(),
                    _idNameGet,
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);

            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, skip, take));
            _mocker
                .Verify<IRequestClient<IFindDepartmentsRequest>>(x =>
                    x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        It.IsAny<object>(), default, default),
                    Times.Once);
            _mocker
                .Verify<IProjectRepository>(x =>
                    x.FindProjects(
                        _findDbProjectsFilter, skip, take, out _totalCount),
                    Times.Once);
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByDepartmentName()
        {
            int skip = 0;
            int take = 2;

            _findProjectsFilter = new FindProjectsFilter
            {
                DepartmentName = DepartmentName
            };
            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                IdNameDepartments = _idNameFind
            };

            _mocker
                .Setup<IFindDbProjectFilterMapper, FindDbProjectsFilter>(x => x.Map(
                    _findProjectsFilter,
                    _idNameFind))
                .Returns(_findDbProjectsFilter);

            _mocker
                .Setup<IProjectRepository, List<DbProject>>(x => x.FindProjects(
                    _findDbProjectsFilter,
                    skip,
                    take,
                    out _totalCount))
                .Returns(_dbProjects);

            _mocker
                .Setup<IFindProjectsResponseMapper, ProjectsResponse>(x => x.Map(
                    _dbProjects,
                    It.IsAny<int>(),
                    _idNameGet,
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);

            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, skip, take));
            _mocker
                .Verify<IRequestClient<IFindDepartmentsRequest>>(x =>
                    x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        It.IsAny<object>(), default, default),
                    Times.Once);
            _mocker
                .Verify<IProjectRepository>(x =>
                    x.FindProjects(
                        _findDbProjectsFilter, skip, take, out _totalCount),
                    Times.Once);
        }

        [Test]
        public void ShouldReturnEmptyListProjectsSearchedByDepartmentNameWhenFindDepartmentRequestIsUnsuccessful()
        {
            var errors = new List<string> { "some error" };

            _brokerFindResponseMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(false);
            _brokerFindResponseMock
                .Setup(x => x.Message.Errors)
                .Returns(errors);

            _mocker
                .Setup<IRequestClient<IFindDepartmentsRequest>, Task<Response<IOperationResult<IFindDepartmentsResponse>>>>(
                    x => x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_brokerFindResponseMock.Object));

            _findProjectsFilter = new FindProjectsFilter
            {
                DepartmentName = DepartmentName
            };
            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                IdNameDepartments = new Dictionary<Guid, string>()
            };

            _mocker
                .Setup<IFindDbProjectFilterMapper, FindDbProjectsFilter>(x => x.Map(
                    _findProjectsFilter,
                    It.Is<Dictionary<Guid, string>>(d => d.Count == 0)))
                .Returns(_findDbProjectsFilter);

            _mocker
                .Setup<IProjectRepository, List<DbProject>>(x => x.FindProjects(
                    _findDbProjectsFilter,
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    out _totalCount))
                .Returns(new List<DbProject>());

            _projectsResponse = new ProjectsResponse
            {
                TotalCount = 1,
                Errors = errors,
                Projects = new List<ProjectInfo>()
            };

            _mocker
                .Setup<IFindProjectsResponseMapper, ProjectsResponse>(x => x.Map(
                    It.IsAny<List<DbProject>>(),
                    It.IsAny<int>(),
                    It.Is<IDictionary<Guid, string>>(d => d.Count == 0),
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);

            var response = _command.Execute(_findProjectsFilter, 0, 2);

            Assert.IsEmpty(response.Projects);
            Assert.IsNotEmpty(response.Errors);
            _mocker.Verify<IRequestClient<IFindDepartmentsRequest>, Task<Response<IOperationResult<IFindDepartmentsResponse>>>>(
                x => x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                    IFindDepartmentsRequest.CreateObj(_findProjectsFilter.DepartmentName, null), default, default), Times.Once);
            _mocker
                .Verify<IProjectRepository>(x =>
                    x.FindProjects(
                        _findDbProjectsFilter, It.IsAny<int>(), It.IsAny<int>(), out _totalCount),
                    Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowEception()
        {
            _mocker
                .Setup<IFindDbProjectFilterMapper, FindDbProjectsFilter>(x => x.Map(
                    _findProjectsFilter,
                    null))
                .Returns(_findDbProjectsFilter);

            _mocker
                .Setup<IProjectRepository, List<DbProject>>(x => x.FindProjects(
                    It.IsAny<FindDbProjectsFilter>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    out _totalCount))
                .Throws(new Exception());

            _mocker
                .Setup<IFindProjectsResponseMapper, ProjectsResponse>(x => x.Map(
                    _dbProjects,
                    It.IsAny<int>(),
                    _idNameGet,
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);

            Assert.Throws<Exception>(() => _command.Execute(_findProjectsFilter, 0, 0));
            _mocker
                .Verify<IRequestClient<IFindDepartmentsRequest>>(x =>
                    x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        It.IsAny<object>(), default, default),
                    Times.Never);
            _mocker
                .Verify<IProjectRepository>(x =>
                    x.FindProjects(
                        It.IsAny<FindDbProjectsFilter>(), It.IsAny<int>(), It.IsAny<int>(), out _totalCount),
                    Times.Once);
        }
    }
}
