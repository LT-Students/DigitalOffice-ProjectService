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
        private Mock<IDepartmentsResponse> _iDerartmentResponseMock;
        private Mock<IOperationResult<IDepartmentsResponse>> _operationResultMock;
        private Mock<Response<IOperationResult<IDepartmentsResponse>>> _brokerResponseMock;
        private Mock<IRequestClient<IFindDepartmentsRequest>> _requestClientMock;

        private int _totalCount;
        private List<DbProject> _dbProjects;
        private List<Guid> _departmentIds;
        private DbProject _dbProject;
        private ProjectsResponse _projectsResponse;
        private FindProjectsFilter _findProjectsFilter;
        private FindDbProjectsFilter _findDbProjectsFilter;

        private void InitializeModels()
        {
            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                ShortName = "N",
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

            _projectsResponse = new ProjectsResponse
            {
                TotalCount = 1,
                Errors = new(),
                Projects = new List<ProjectInfo>
                {
                    new ProjectInfo
                    {
                        Id =_dbProject.Id,
                        Name = _dbProject.Name,
                        ShortName = _dbProject.ShortName,
                        ShortDescription = _dbProject.ShortDescription,
                        DepartmentInfo = new DepartmentInfo
                        {
                            Id = Guid.NewGuid(),
                            Name = "DepartmentName"
                        }
                    }
                }
            };

            _findProjectsFilter = new FindProjectsFilter
            {
                Name = "Name"
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
                    It.IsAny<string>(),
                    It.IsAny<List<string>>()))
                .Returns(_projectsResponse);
            _mocker
                .Setup<IFindDbProjectFilterMapper, FindDbProjectsFilter>(x => x.Map(
                    _findProjectsFilter,
                    It.IsAny<List<Guid>>()))
                .Returns(_findDbProjectsFilter);

            BrokerSetUp();

            _command = _mocker.CreateInstance<FindProjectsCommand>();
        }

        private void BrokerSetUp()
        {
            _iDerartmentResponseMock = new Mock<IDepartmentsResponse>();
            _iDerartmentResponseMock
                .Setup(x => x.DepartmentIds)
                .Returns(_departmentIds);

            _operationResultMock = new Mock<IOperationResult<IDepartmentsResponse>>();
            _operationResultMock
                .Setup(x => x.IsSuccess)
                .Returns(true);
            _operationResultMock
                .Setup(x => x.Errors)
                .Returns(new List<string>());
            _operationResultMock
                .Setup(x => x.Body)
                .Returns(_iDerartmentResponseMock.Object);

            _brokerResponseMock = new Mock<Response<IOperationResult<IDepartmentsResponse>>>();
            _brokerResponseMock
                .Setup(x => x.Message)
                .Returns(_operationResultMock.Object);

            _requestClientMock = new Mock<IRequestClient<IFindDepartmentsRequest>>();
            _requestClientMock
                .Setup(x => x.GetResponse<IOperationResult<IDepartmentsResponse>>(
                    It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_brokerResponseMock.Object));

            _mocker
                .Setup<Request<IRequestClient<IFindDepartmentsRequest>>, Task>(x => x.Task)
                .Returns(Task.FromResult(_requestClientMock.Object));
        }

        [SetUp]
        public void SetUp()
        {
            InitializeModels();
            SetUpMocks();
        }

        [Test]
        public void ShouldReturnFoundProjectInfos()
        {
            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, 0, 0));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFilterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _command.Execute(null, 0, 0));
        }

        [Test]
        public void ShouldReturnFoundProjectInfosByDepartmentName()
        {
            _findProjectsFilter = new FindProjectsFilter
            {
                DepartmentName = "DepartmentName"
            };
            _findDbProjectsFilter = new FindDbProjectsFilter
            {
                List<Guid> =
            };
            SerializerAssert.AreEqual(_projectsResponse, _command.Execute(_findProjectsFilter, 0, 0));
        }
    }
}
