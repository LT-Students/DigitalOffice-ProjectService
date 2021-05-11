using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
{
    internal class GetProjectCommandTests
    {
        private IGetProjectCommand _command;
        private Mock<ILogger<GetProjectCommand>> _loggerMock;
        private Mock<IProjectRepository> _repositoryMock;
        private Mock<IProjectExpandedResponseMapper> _projectExpandedResponseMapperMock;
        private Mock<IProjectUserInfoMapper> _projectUserInfoMapperMock;
        private Mock<IProjectFileInfoMapper> _projectFileInfoMapperMock;
        private Mock<IDepartmentInfoMapper> _departmentInfoMapperMock;
        
        private Mock<IRequestClient<IGetDepartmentRequest>> _rcDepartmentMock;

        private Mock<IRequestClient<IGetUsersDataRequest>> _rcUsersDataMock;

        private List<UserData> _usersData;
        private List<ProjectUserInfo> _projectUsersInfo;
        private List<ProjectFileInfo> _projectFilesInfo;
        private DbProjectUser _dbProjectUser;
        private DbProjectFile _dbProjectFile;
        private DbProject _dbProject;
        private DepartmentInfo _departmentInfo;
        private GetProjectFilter _fullFilter;
        private ProjectInfo _projectInfo;
        private ProjectExpandedResponse _expectedResponse;

        private Guid _projectId;

        private static Mock<IRequestClient<TRequest>> SuccessBrokerSetUp<TRequest, TResponse>(Mock<TResponse> responseMock) where TRequest : class where TResponse : class
        {
            var operationResultMock = new Mock<IOperationResult<TResponse>>();
            operationResultMock
                .Setup(x => x.Body)
                .Returns(responseMock.Object);
            operationResultMock
                .Setup(x => x.IsSuccess)
                .Returns(true);
            operationResultMock
                .Setup(x => x.Errors)
                .Returns(new List<string>());

            var brokerResponseMock = new Mock<Response<IOperationResult<TResponse>>>();
            brokerResponseMock
                .Setup(x => x.Message)
                .Returns(operationResultMock.Object);

            var requestClientMock = new Mock<IRequestClient<TRequest>>();
            requestClientMock
                .Setup(x => x.GetResponse<IOperationResult<TResponse>>(It.IsAny<object>(), default, default).Result)
                .Returns(brokerResponseMock.Object);

            return requestClientMock;
        }

        private void DepartmentBrokerSetUp()
        {
            var departmentResponseMock = new Mock<IGetDepartmentResponse>();
            departmentResponseMock
                .Setup(x => x.Id)
                .Returns(_departmentInfo.Id);
            departmentResponseMock
                .Setup(x => x.Name)
                .Returns(_departmentInfo.Name);

            _rcDepartmentMock = SuccessBrokerSetUp<IGetDepartmentRequest, IGetDepartmentResponse>(departmentResponseMock);
        }

        private void UsersBrokerSetUp()
        {
            var usersResponseMock = new Mock<IGetUsersDataResponse>();
            usersResponseMock
                .Setup(x => x.UsersData)
                .Returns(_usersData);

            _rcUsersDataMock = SuccessBrokerSetUp<IGetUsersDataRequest, IGetUsersDataResponse>(usersResponseMock);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectId = Guid.NewGuid();

            _dbProject = new DbProject
            {
                Id = _projectId,
                Name = "Project",
                DepartmentId = Guid.NewGuid()
            };

            _departmentInfo = new DepartmentInfo
            {
                Id = _dbProject.DepartmentId,
                Name = "DepartmentName"
            };

            _projectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                Name = _dbProject.Name,
                Department = _departmentInfo
            };

            _dbProjectUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                ProjectId = _projectId,
                Project = _dbProject,
                UserId = Guid.NewGuid(),
                AddedOn = DateTime.Now,
                RemovedOn = DateTime.Now,
                IsActive = true,
                Role = (int)UserRoleType.ProjectAdmin
            };

            _dbProject.Users.Add(_dbProjectUser);

            _usersData = new List<UserData>
            {
                new UserData
                {
                    Id = _dbProjectUser.Id,
                    IsActive = _dbProjectUser.IsActive,
                    FirstName = "Spartak",
                    LastName = "Ryabtsev",
                    MiddleName = "Alexandrovich"
                }
            };

            _projectUsersInfo = new List<ProjectUserInfo>
            {
                new ProjectUserInfo
                {
                    Id = _usersData.First().Id,
                    Role = (UserRoleType)_dbProjectUser.Role,
                    FirstName = _usersData.First().FirstName,
                    LastName = _usersData.First().LastName,
                    MiddleName = _usersData.First().MiddleName,
                    AddedOn = _dbProjectUser.AddedOn,
                    RemovedOn = _dbProjectUser.RemovedOn,
                    IsActive = _dbProjectUser.IsActive
                }
            };

            _dbProjectFile = new DbProjectFile
            {
                Id = Guid.NewGuid(),
                ProjectId = _projectId,
                Project = _dbProject
            };

            _dbProject.Files.Add(_dbProjectFile);

            _projectFilesInfo = new List<ProjectFileInfo>
            {
                new ProjectFileInfo
                {
                    FileId = _dbProjectFile.Id,
                    ProjectId = _projectId
                }
            };

            _fullFilter = new GetProjectFilter
            {
                ProjectId = _projectId,
                IncludeFiles = true,
                IncludeUsers = true,
                ShowNotActiveUsers = true
            };

            _expectedResponse = new ProjectExpandedResponse
            {
                Project = _projectInfo,
                Files = _projectFilesInfo,
                Users = _projectUsersInfo
            };
        }

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<GetProjectCommand>>();

            _repositoryMock = new Mock<IProjectRepository>();
            _repositoryMock
                .Setup(x => x.GetProject(_fullFilter))
                .Returns(_dbProject);

            DepartmentBrokerSetUp();
            UsersBrokerSetUp();

            _departmentInfoMapperMock = new Mock<IDepartmentInfoMapper>();
            _departmentInfoMapperMock
                .Setup(x => x.Map(It.IsAny<IGetDepartmentResponse>()))
                .Returns(_departmentInfo);

            _projectUserInfoMapperMock = new Mock<IProjectUserInfoMapper>();
            _projectUserInfoMapperMock
                .Setup(x => x.Map(_usersData.First(), _dbProjectUser))
                .Returns(_projectUsersInfo.First());

            _projectFileInfoMapperMock = new Mock<IProjectFileInfoMapper>();
            _projectFileInfoMapperMock
                .Setup(x => x.Map(_dbProjectFile))
                .Returns(_projectFilesInfo.First());

            _projectExpandedResponseMapperMock = new Mock<IProjectExpandedResponseMapper>();
            _projectExpandedResponseMapperMock
                .Setup(x => x.Map(It.IsAny<DbProject>(), It.IsAny<List<ProjectUserInfo>>(), It.IsAny<List<ProjectFileInfo>>(), It.IsAny<DepartmentInfo>(), It.IsAny<List<string>>()))
                .Returns(_expectedResponse)
                .Verifiable();

            _command = new GetProjectCommand(
                _loggerMock.Object,
                _repositoryMock.Object,
                _projectExpandedResponseMapperMock.Object,
                _projectUserInfoMapperMock.Object,
                _projectFileInfoMapperMock.Object,
                _departmentInfoMapperMock.Object,
                _rcDepartmentMock.Object,
                _rcUsersDataMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenGetProjectThrowsIt()
        {
            _repositoryMock
                .Setup(x => x.GetProject(_fullFilter))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_fullFilter));
        }

        [Test]
        public void ShouldNotThrowExceptionWhenGetDepartmentClientThrowsIt()
        {
            _rcDepartmentMock
                .Setup(x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(It.IsAny<object>(), default, default).Result)
                .Throws(new Exception());

            var result = _command.Execute(_fullFilter);

            SerializerAssert.AreEqual(_expectedResponse, result);
        }

        [Test]
        public void ShouldNotThrowExceptionWhenGetUsersClientThrowsIt()
        {
            _rcUsersDataMock
                .Setup(x => x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default).Result)
                .Throws(new Exception());

            var result = _command.Execute(_fullFilter);

            SerializerAssert.AreEqual(_expectedResponse, result);
        }

        [Test]
        public void ShouldNotThrowExceptionWhenProjectUserMapperThrowsIt()
        {
            _projectUserInfoMapperMock
                .Setup(x => x.Map(It.IsAny<UserData>(), It.IsAny<DbProjectUser>()))
                .Throws(new Exception());

            var result = _command.Execute(_fullFilter);

            SerializerAssert.AreEqual(_expectedResponse, result);
        }

        [Test]
        public void ShouldThrowExceptionWhenProjectUserFileMapperThrowsIt()
        {
            _projectFileInfoMapperMock
                .Setup(x => x.Map(_dbProjectFile))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_fullFilter));
        }

        [Test]
        public void ShouldThrowExceptionWhenProjectExpandedResponseMapperThrowsIt()
        {
            _projectExpandedResponseMapperMock
                .Setup(x => x.Map(It.IsAny<DbProject>(), It.IsAny<List<ProjectUserInfo>>(), It.IsAny<List<ProjectFileInfo>>(), It.IsAny<DepartmentInfo>(), It.IsAny<List<string>>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_fullFilter));
        }

        [Test]
        public void ShouldReturnProjectInfo()
        {
            _projectExpandedResponseMapperMock
                .Setup(x => x.Map(It.IsAny<DbProject>(), It.IsAny<List<ProjectUserInfo>>(), It.IsAny<List<ProjectFileInfo>>(), It.IsAny<DepartmentInfo>(), It.IsAny<List<string>>()))
                .Returns(_expectedResponse)
                .Verifiable();

            var result = _command.Execute(_fullFilter);

            _projectExpandedResponseMapperMock.Verify(x => x.Map(It.IsAny<DbProject>(), It.IsAny<List<ProjectUserInfo>>(), It.IsAny<List<ProjectFileInfo>>(), It.IsAny<DepartmentInfo>(), It.IsAny<List<string>>()), Times.Once);

            SerializerAssert.AreEqual(_expectedResponse, result);
        }

        [Test]
        public void ShouldReturnProjectInfoWithoutInActiveUsers()
        {
            var filterWithoutNotActiveUsers = new GetProjectFilter
            {
                ProjectId = _projectId,
                IncludeFiles = true,
                IncludeUsers = true,
                ShowNotActiveUsers = false
            };

            _repositoryMock
                .Setup(x => x.GetProject(filterWithoutNotActiveUsers))
                .Returns(_dbProject);

            _dbProjectUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                ProjectId = _projectId,
                Project = _dbProject,
                UserId = Guid.NewGuid(),
                AddedOn = DateTime.Now,
                RemovedOn = DateTime.Now,
                IsActive = false,
                Role = (int)UserRoleType.ProjectAdmin
            };

            _dbProject.Users = new List<DbProjectUser> { _dbProjectUser };

            var result = _command.Execute(_fullFilter);

            SerializerAssert.AreEqual(_expectedResponse, result);
        }
    }
}