using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    public class GetTaskCommandTests
    {
        #region Private

        #region Private Models

        private IGetTaskCommand _command;

        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IAccessValidator> _accessValidatorMock;
        private Mock<IHttpContextAccessor> _httpAccessorMock;
        private TaskResponseMapper _mapper;
        private Mock<ILogger<GetTaskCommand>> _loggerMock;

        private Mock<IRequestClient<IGetDepartmentRequest>> _requestClient;
        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _operationResultBrokerMock;

        private DbTask _dbTask;
        
        #endregion

        #region Private Fields

        private readonly string _name = "NewName";
        private readonly string _description = "New Description";
        private readonly Guid _assignedTo = Guid.NewGuid();
        private readonly int _plannedMinutes = 60;
        private readonly Guid _priorityId = Guid.NewGuid();
        private readonly Guid _statusId = Guid.NewGuid();
        private readonly Guid _typeId = Guid.NewGuid();
        private readonly Guid _projectId = Guid.NewGuid();
        private readonly Guid _departmentId = Guid.NewGuid();
        private readonly Guid _taskId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        private readonly OperationResultResponse<bool> _fullSuccessModel = new()
        {
            Status = OperationResultStatusType.FullSuccess,
            Body = true,
            Errors = new List<string>()
        };

        #endregion

        #region Private Methods

        private void VerifyCalls(
            Func<Times> userRepositoryTimes,
            Func<Times> getInTaskRepositoryTimes,
            Func<Times> getFullInTaskRepositoryTimes,
            Func<Times> requestClientTimes,
            Func<Times> httpAccessorTimes)
        {
            _userRepositoryMock.Verify(x => x.GetProjectUsers(
                It.IsAny<Guid>(), It.IsAny<bool>()), userRepositoryTimes);

            _taskRepositoryMock.Verify(x =>
                x.Get(It.IsAny<Guid>()), getInTaskRepositoryTimes);

            _taskRepositoryMock.Verify(x =>
                x.GetFullModel(It.IsAny<Guid>()), getFullInTaskRepositoryTimes);

            _requestClient.Verify(x =>
                x.GetResponse<IOperationResult<IGetDepartmentResponse>>(It.IsAny<object>(),
                    default,
                    default), requestClientTimes);

            _httpAccessorMock.Verify(x => x.HttpContext, httpAccessorTimes);
        }

        private void ClientRequestUp(Guid newGuid)
        {
            IDictionary<object, object> httpContextItems = new Dictionary<object, object>();

            httpContextItems.Add("UserId", newGuid);

            _httpAccessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(httpContextItems)
                .Verifiable();
        }

        private void RcGetDepartment(Guid userId)
        {
            var department = new Mock<IGetDepartmentResponse>();
            department.Setup(x => x.DirectorUserId).Returns(userId);

            _operationResultBrokerMock
                .Setup(x => x.Message.Body)
                .Returns(department.Object);

            _operationResultBrokerMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(true);

            _operationResultBrokerMock
                .Setup(x => x.Message.Errors)
                .Returns(new List<string>());

            _requestClient
                .Setup(x =>
                    x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                        It.IsAny<object>(),
                        default,
                        default))
                .Returns(Task.FromResult(_operationResultBrokerMock.Object))
                .Verifiable();
        }

        #endregion

        #endregion

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbTask = new DbTask
            {
                ProjectId = _projectId,
                AssignedTo = _assignedTo,
                AssignedUser = new DbProjectUser(),
                Author = new DbProjectUser(),
                CreatedAt = new DateTime(),
                AuthorId = _assignedTo,
                Description = "Description",
                Id = _taskId,
                Name = "Name",
                Number = 1
            };
        }

        [SetUp]
        public void SetUp()
        {
            #region Mock initialization

            _taskRepositoryMock = new Mock<ITaskRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _accessValidatorMock = new Mock<IAccessValidator>();
            _httpAccessorMock = new Mock<IHttpContextAccessor>();
            _mapper = new TaskResponseMapper();
            _loggerMock = new Mock<ILogger<GetTaskCommand>>();
            _requestClient = new Mock<IRequestClient<IGetDepartmentRequest>>();
            _operationResultBrokerMock = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();

            #endregion

            ClientRequestUp(Guid.NewGuid());
            RcGetDepartment(Guid.NewGuid());

            #region Mock default setups

            _taskRepositoryMock
                .Setup(x => x.GetFullModel(_taskId))
                .Returns(_dbTask).Verifiable();

            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(false);

            _userRepositoryMock
                .Setup(x => x.GetProjectUsers(_projectId, false))
                .Returns(new List<DbProjectUser>()
                {
                    new DbProjectUser
                    {
                        UserId = Guid.NewGuid()
                    }
                }).Verifiable();

            #endregion

            _command = new GetTaskCommand(
                _taskRepositoryMock.Object,
                _projectRepositoryMock.Object,
                _userRepositoryMock.Object,
                _accessValidatorMock.Object,
                _httpAccessorMock.Object,
                _mapper,
                _loggerMock.Object,
                _requestClient.Object);
        }
        
        [Test]
        public void FullSuccessOperationWhenUserIsAdmin()
        {
            _accessValidatorMock.Setup(x => x.IsAdmin(null)).Returns(true);

            SerializerAssert.AreEqual(_dbTask, _command.Execute(_taskId));

            VerifyCalls(Times.Once,
                Times.Once,
                Times.Once,
                Times.Once,
                Times.Once);
        }

        [Test]
        public void FullSuccessOperationWhenUserInProject()
        {
            ClientRequestUp(_userId);
            
            _userRepositoryMock
                .Setup(x => x.GetProjectUsers(_projectId, false))
                .Returns(new List<DbProjectUser>()
                {
                    new DbProjectUser
                    {
                        UserId = _userId
                    }
                }).Verifiable();
            
            SerializerAssert.AreEqual(_dbTask, _command.Execute(_taskId));

            VerifyCalls(Times.Once,
                Times.Once,
                Times.Once,
                Times.Once,
                Times.Once);
        }

        [Test]
        public void FullSuccessWhenUserIsDepartmentDirector()
        {
            ClientRequestUp(_userId);

            RcGetDepartment(_userId);
            
            SerializerAssert.AreEqual(_dbTask, _command.Execute(_taskId));

            VerifyCalls(Times.Once,
                Times.Once,
                Times.Once,
                Times.Once,
                Times.Once);
        }

        [Test]
        public void ExceptionWhenUserCannotEdit()
        {
            Assert.Throws<ForbiddenException>(() => _command.Execute(_taskId));

            VerifyCalls(Times.Once,
                Times.Once,
                Times.Never,
                Times.Once,
                Times.Once);
        }

        [Test]
        public void ExceptionWhenRequestClientUnavailable()
        {
            _accessValidatorMock.Setup(x => x.IsAdmin(null)).Returns(true);

            _requestClient
                .Setup(x =>
                    x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                        It.IsAny<object>(),
                        default,
                        default))
                .Returns((Task<Response<IOperationResult<IGetDepartmentResponse>>>) null);

            Assert.AreEqual(1, _command.Execute(_taskId).Errors.Count);

            VerifyCalls(Times.Once,
                Times.Once,
                Times.Once,
                Times.Once,
                Times.Once);
        }
    }
}