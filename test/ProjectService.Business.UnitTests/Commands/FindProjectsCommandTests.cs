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
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;
using System;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using System.Threading.Tasks;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.ProjectService.Business.Commands;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class FindProjectsCommandTests
    {
        private IFindProjectsCommand _command;
        private AutoMocker _mocker;
        private Mock<Response<IOperationResult<IFindDepartmentsResponse>>> _brokerFindResponseMock;
        private Mock<Response<IOperationResult<IGetDepartmentsNamesResponse>>> _brokerGetResponseMock;

        private int _totalCount;
        private List<DbProject> _dbProjects;
        private Dictionary<Guid, string> _idNameFind;
        private Dictionary<Guid, string> _idNameGet;
        private DbProject _dbProject;
        private ProjectsResponse _projectsResponse;
        private FindProjectsFilter _findProjectsFilter;
        private FindDbProjectsFilter _findDbProjectsFilter;
        private ProjectInfo _projectInfo;

        private const string _name = "Name";
        private const string _shortName = "ShortName";
        private const string _departmentName = "DepartmentName";
        private Guid _departmentId;

        #region Setup

        public void SetUpModels()
        {
            _departmentId = Guid.NewGuid();

            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<FindProjectsCommand>();

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = _name,
                ShortName = _shortName,
                ShortDescription = "description",
                DepartmentId = _departmentId,
                CreatedAt = DateTime.UtcNow
            };

            _idNameFind = new();
            _idNameFind.Add(_departmentId, _departmentName);

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
                DepartmentInfo = new DepartmentInfo
                {
                    Id = _departmentId,
                    Name = _departmentName
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
            _idNameGet.Add(_dbProject.DepartmentId, _departmentName);

            _findProjectsFilter = new FindProjectsFilter
            {
                Name = _name
            };

            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                Name = _findProjectsFilter.Name
            };

            _brokerFindResponseMock = new Mock<Response<IOperationResult<IFindDepartmentsResponse>>>();
            _brokerFindResponseMock
                .Setup(x => x.Message.Body.IdNamePairs)
                .Returns(_idNameFind);

            _brokerGetResponseMock = new Mock<Response<IOperationResult<IGetDepartmentsNamesResponse>>>();
            _brokerGetResponseMock
                .Setup(x => x.Message.Body.IdNamePairs)
                .Returns(_idNameGet);
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

            _brokerGetResponseMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(true);
            _brokerGetResponseMock
                .Setup(x => x.Message.Errors)
                .Returns(null as List<string>);

            _mocker
                .Setup<IRequestClient<IFindDepartmentsRequest>, Task<Response<IOperationResult<IFindDepartmentsResponse>>>>(
                    x => x.GetResponse<IOperationResult<IFindDepartmentsResponse>>(
                        IFindDepartmentsRequest.CreateObj(_departmentName), default, default))
                .Returns(Task.FromResult(_brokerFindResponseMock.Object));

            //_mocker
            //    .Setup<IRequestClient<IGetDepartmentsNamesRequest>, Task<Response<IOperationResult<IGetDepartmentsNamesResponse>>>>(
            //        x => x.GetResponse<IOperationResult<IGetDepartmentsNamesResponse>>(
            //            IGetDepartmentsNamesRequest.CreateObj(
            //                It.IsAny<IList<Guid>>()), default, default))
            //    .Returns(Task.FromResult(_brokerGetResponseMock.Object));
            _mocker
                .Setup<IRequestClient<IGetDepartmentsNamesRequest>, Task<Response<IOperationResult<IGetDepartmentsNamesResponse>>>>(
                    x => x.GetResponse<IOperationResult<IGetDepartmentsNamesResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_brokerGetResponseMock.Object));
        }

        #endregion

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFilterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _command.Execute(null, 0, 0));
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
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByShortName()
        {
            _findProjectsFilter = new FindProjectsFilter
            {
                ShortName = _shortName
            };
            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                ShortName = _shortName
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
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByDepartmentName()
        {
            int skip = 0;
            int take = 2;

            _findProjectsFilter = new FindProjectsFilter
            {
                DepartmentName = _departmentName
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
                        IFindDepartmentsRequest.CreateObj(_departmentName), default, default))
                .Returns(Task.FromResult(_brokerFindResponseMock.Object));

            _findProjectsFilter = new FindProjectsFilter
            {
                DepartmentName = _departmentName
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
                    IFindDepartmentsRequest.CreateObj(_findProjectsFilter.DepartmentName), default, default), Times.Once);
        }

        [Test]
        public void ShouldReturnResponseWithoutDepartmentsNamesIfGetDepartmentsNamesRequestIsUnsuccessful()
        {
            int take = 2, skip = 0;
            var errors = new List<string> { "some error" };

            _brokerGetResponseMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(false);
            _brokerGetResponseMock
                .Setup(x => x.Message.Errors)
                .Returns(errors);

            //_mocker
            //    .Setup<IRequestClient<IGetDepartmentsNamesRequest>, Task<Response<IOperationResult<IGetDepartmentsNamesResponse>>>>(
            //        x => x.GetResponse<IOperationResult<IGetDepartmentsNamesResponse>>(
            //            IGetDepartmentsNamesRequest.CreateObj(It.Is<List<Guid>>(l => l.Count == 1 && l.Contains(_departmentId))), default, default))
            //    .Returns(Task.FromResult(_brokerGetResponseMock.Object));

            _projectInfo.DepartmentInfo.Name = null;

            _projectsResponse = new ProjectsResponse
            {
                TotalCount = 1,
                Errors = null,
                Projects = new List<ProjectInfo>
                {
                    _projectInfo
                }
            };

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
                    It.Is<IDictionary<Guid, string>>(d => d.Count == 0),
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);

            var response = _command.Execute(_findProjectsFilter, skip, take);

            SerializerAssert.AreEqual(_projectsResponse, response);
            //_mocker.Verify<IRequestClient<IGetDepartmentsNamesRequest>, Task<Response<IOperationResult<IGetDepartmentsNamesResponse>>>>(
            //    x => x.GetResponse<IOperationResult<IGetDepartmentsNamesResponse>>(
            //        IGetDepartmentsNamesRequest.CreateObj(
            //            It.Is<List<Guid>>(l => l.Count == 1 && l.Contains(_departmentId))), default, default), Times.Once);
        }
    }
}
