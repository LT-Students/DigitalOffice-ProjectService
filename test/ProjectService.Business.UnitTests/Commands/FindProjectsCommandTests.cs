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

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class FindProjectsCommandTests
    {
        private IFindProjectsCommand _command;
        private AutoMocker _mocker;
        private Mock<Response<IOperationResult<IDepartmentsResponse>>> _brokerFindResponseMock;
        private Mock<Response<IOperationResult<IGetDepartmentsNamesResponse>>> _brokerGetResponseMock;

        private int _totalCount;
        private List<DbProject> _dbProjects;
        private List<Guid> _departmentIds;
        private Dictionary<Guid, string> _idNameDepartment;
        private DbProject _dbProject;
        private ProjectsResponse _projectsResponse;
        private FindProjectsFilter _findProjectsFilter;
        private FindDbProjectsFilter _findDbProjectsFilter;
        private ProjectInfo _foundProjectInfo;

        private const string _name = "Name";
        private const string _shortName = "ShortName";
        private const string _departmentName = "DepartmentName";
        private void InitializeModels()
        {
            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = _name,
                ShortName = _shortName,
                Description = "description",
                DepartmentId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            _departmentIds = new List<Guid>
            {
                _dbProject.DepartmentId
            };

            _dbProjects = new List<DbProject>
            {
                _dbProject
            };

            _foundProjectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name,
                ShortName = _dbProject.ShortName,
                ShortDescription = _dbProject.ShortDescription,
                DepartmentInfo = new DepartmentInfo
                {
                    Id = Guid.NewGuid(),
                    Name = _departmentName
                }
            };

            _projectsResponse = new ProjectsResponse
            {
                TotalCount = 1,
                Errors = new(),
                Projects = new List<ProjectInfo>
                {
                    _foundProjectInfo
                }
            };

            _idNameDepartment = new Dictionary<Guid, string>();
            _idNameDepartment.Add(_dbProject.DepartmentId, _departmentName);

            _findProjectsFilter = new FindProjectsFilter
            {
                Name = _name
            };

            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                Name = _findProjectsFilter.Name
            };
        }

        private void SetUpMocks()
        {
            _mocker = new AutoMocker();
            _mocker
                .Setup<IProjectRepository, List<DbProject>>(x => x.FindProjects(
                    It.IsAny<FindDbProjectsFilter>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    out _totalCount))
                .Returns(_dbProjects);
            _mocker
                .Setup<IFindProjectsResponseMapper, ProjectsResponse>(x => x.Map(
                    _dbProjects,
                    It.IsAny<int>(),
                    _idNameDepartment,
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);
            _mocker
                .Setup<IFindDbProjectFilterMapper, FindDbProjectsFilter>(x => x.Map(
                    It.IsAny<FindProjectsFilter>(),
                    It.IsAny<List<Guid>>()))
                .Returns(_findDbProjectsFilter);

            BrokerSetUp();

            _command = _mocker.CreateInstance<FindProjectsCommand>();
        }

        private void BrokerSetUp()
        {
            _brokerFindResponseMock = new Mock<Response<IOperationResult<IDepartmentsResponse>>>();
            _brokerFindResponseMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(true);
            _brokerFindResponseMock
                .Setup(x => x.Message.Errors)
                .Returns(null as List<string>);
            _brokerFindResponseMock
                .Setup(x => x.Message.Body.DepartmentIds)
                .Returns(_departmentIds);

            _brokerGetResponseMock = new Mock<Response<IOperationResult<IGetDepartmentsNamesResponse>>>();
            _brokerGetResponseMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(true);
            _brokerGetResponseMock
                .Setup(x => x.Message.Errors)
                .Returns(null as List<string>);
            _brokerGetResponseMock
                .Setup(x => x.Message.Body.IdNamePairs)
                .Returns(_idNameDepartment);

            _mocker
                .Setup<IRequestClient<IFindDepartmentsRequest>, Task<Response<IOperationResult<IDepartmentsResponse>>>>(
                    x => x.GetResponse<IOperationResult<IDepartmentsResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_brokerFindResponseMock.Object));

            _mocker
                .Setup<IRequestClient<IGetDepartmentsNamesRequest>, Task<Response<IOperationResult<IGetDepartmentsNamesResponse>>>>(
                    x => x.GetResponse<IOperationResult<IGetDepartmentsNamesResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_brokerGetResponseMock.Object));
        }

        [SetUp]
        public void SetUp()
        {
            InitializeModels();
            SetUpMocks();
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByName()
        {
            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, 0, 0));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFilterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _command.Execute(null, 0, 0));
        }

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
            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, 0, 0));
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByDepartmentName()
        {
            _findProjectsFilter = new FindProjectsFilter
            {
                DepartmentName = _departmentName
            };
            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                DepartmentIds = _departmentIds
            };
            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, 0, 0));
        }

        [Test]
        public void ShouldReturnEmptyListProjectsWhenFindDepartmentRequestIsUnsuccessful()
        {
            _brokerFindResponseMock = new Mock<Response<IOperationResult<IDepartmentsResponse>>>();
            _brokerFindResponseMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(false);
            _brokerFindResponseMock
                .Setup(x => x.Message.Errors)
                .Returns(new List<string>());

            _mocker
                .Setup<IRequestClient<IFindDepartmentsRequest>, Task<Response<IOperationResult<IDepartmentsResponse>>>>(
                    x => x.GetResponse<IOperationResult<IDepartmentsResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_brokerFindResponseMock.Object));

            _findProjectsFilter = new FindProjectsFilter
            {
                DepartmentName = _departmentName
            };
            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                DepartmentIds = _departmentIds
            };

            _mocker
                .Setup<IProjectRepository, List<DbProject>>(x => x.FindProjects(
                    It.IsAny<FindDbProjectsFilter>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    out _totalCount))
                .Returns(new List<DbProject>());

            _projectsResponse = new ProjectsResponse
            {
                TotalCount = 1,
                Errors = new(),
                Projects = new List<ProjectInfo>()
            };

            _mocker
                .Setup<IFindProjectsResponseMapper, ProjectsResponse>(x => x.Map(
                    It.IsAny<List<DbProject>>(),
                    It.IsAny<int>(),
                    It.IsAny<IDictionary<Guid, string>>(),
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);

            var response = _command.Execute(_findProjectsFilter, 0, 0);
            Assert.IsEmpty(response.Projects);
        }
    }
}
