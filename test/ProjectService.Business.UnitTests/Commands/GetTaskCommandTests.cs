using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    public class GetTaskCommandTests
    {
        #region Private Models

        private IGetTaskCommand _command;

        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IAccessValidator> _accessValidatorMock;
        private Mock<IHttpContextAccessor> _httpAccessorMock;
        private Mock<ITaskResponseMapper> _taskResponseMapperMock;
        private Mock<ITaskInfoMapper> _taskInfoMapper;
        
        private Mock<ILogger<GetTaskCommand>> _loggerMock;

        private Mock<IRequestClient<IGetDepartmentRequest>> _departmentRequestClient;
        private Mock<IRequestClient<IGetUsersDataRequest>> _userRequestClient;
        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _operationResultBrokerMock;
        private Mock<Response<IOperationResult<IGetUsersDataResponse>>> _operationResultUserBrokerMock;
        
        private JsonPatchDocument<EditTaskRequest> _request;

        #endregion

        #region Private Fields

        private readonly string _name = "NewName";
        private readonly string _description = "New Description";
        private readonly int _plannedMinutes = 60;
        private readonly Guid _projectId = Guid.NewGuid();
        private readonly Guid _taskId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        private readonly OperationResultResponse<TaskResponse> _fullSuccessModel = new()
        {
            Body = new TaskResponse(),
            Errors = new List<string>(),
            Status = OperationResultStatusType.FullSuccess
        };

        #endregion

        private void ClientRequestUp(Guid newGuid)
        {
            IDictionary<object, object> httpContextItems = new Dictionary<object, object>();

            httpContextItems.Add("UserId", newGuid);

            _httpAccessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(httpContextItems)
                .Verifiable();
        }

        private void RcGetDepartment(Guid departmentId)
        {
            var department = new Mock<IGetDepartmentResponse>();
            department.Setup(x => x.DirectorUserId).Returns(_userId);

            _operationResultBrokerMock
                .Setup(x => x.Message.Body)
                .Returns(department.Object);

            _operationResultBrokerMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(true);

            _operationResultBrokerMock
                .Setup(x => x.Message.Errors)
                .Returns(new List<string>());

            _departmentRequestClient
                .Setup(x =>
                    x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                        It.IsAny<object>(),
                        default,
                        default))
                .Returns(Task.FromResult(_operationResultBrokerMock.Object))
                .Verifiable();
        }

        private void RcGetUser()
        {
            var users = new Mock<IGetUsersDataResponse>();
            users
                .Setup(x => x.UsersData)
                .Returns(new List<UserData>());

            _operationResultUserBrokerMock
                .Setup(x => x.Message.Body)
                .Returns(users.Object);
            
            _operationResultUserBrokerMock
                .Setup(x => x.Message.IsSuccess)
                .Returns(true);

            _operationResultUserBrokerMock
                .Setup(x => x.Message.Errors)
                .Returns(new List<string>());

            _userRequestClient
                .Setup(x => 
                    x.GetResponse<IOperationResult<IGetUsersDataResponse>>(
                        It.IsAny<object>(),
                        default,
                        default)
                )
                .Returns(Task.FromResult(_operationResultUserBrokerMock.Object))
                .Verifiable();
        }

        [SetUp]
        public void SetUp()
        {
            #region Mock initialization

            _taskRepositoryMock = new Mock<ITaskRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _accessValidatorMock = new Mock<IAccessValidator>();
            _httpAccessorMock = new Mock<IHttpContextAccessor>();
            _taskResponseMapperMock = new Mock<ITaskResponseMapper>();
            _taskInfoMapper = new Mock<ITaskInfoMapper>();
            _loggerMock = new Mock<ILogger<GetTaskCommand>>();
            _departmentRequestClient = new Mock<IRequestClient<IGetDepartmentRequest>>();
            _userRequestClient = new Mock<IRequestClient<IGetUsersDataRequest>>();
            _operationResultBrokerMock = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
            _operationResultUserBrokerMock = new Mock<Response<IOperationResult<IGetUsersDataResponse>>>();
            #endregion
            
            #region Mock default setups

            _taskResponseMapperMock.Setup(x => x.Map(
                    It.IsAny<DbTask>(),
                    It.IsAny<UserData>(),
                    It.IsAny<UserData>(),
                    It.IsAny<UserData>(),
                    It.IsAny<string>(),
                    It.IsAny<UserData>(),
                    It.IsAny<ICollection<TaskInfo>>()
                )
            ).Returns(new TaskResponse());

            _taskRepositoryMock
                .Setup(x => x.Get(_taskId, false))
                .Returns(new DbTask
                {
                    ProjectId = _projectId
                }).Verifiable();
            
            _taskRepositoryMock
                .Setup(x => x.Get(_taskId, true))
                .Returns(new DbTask
                {
                    ProjectId = _projectId
                }).Verifiable();

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
            
            ClientRequestUp(Guid.NewGuid());
            RcGetDepartment(Guid.NewGuid());
            RcGetUser();

            _command = new GetTaskCommand(
                _taskRepositoryMock.Object,
                _userRepositoryMock.Object,
                _accessValidatorMock.Object,
                _httpAccessorMock.Object,
                _taskResponseMapperMock.Object,
                _taskInfoMapper.Object,
                _loggerMock.Object,
                _departmentRequestClient.Object,
                _userRequestClient.Object
            );
        }

        [Test]
        public void FullSuccess()
        {
            _accessValidatorMock.Setup(x => x.IsAdmin(It.IsAny<Guid>())).Returns(true);

            var result = _command.Execute(_taskId);
            SerializerAssert.AreEqual(_fullSuccessModel, result);
        }

        [Test]
        public void Forbidden()
        {
            Assert.Throws<ForbiddenException>(() => _command.Execute(_taskId));
        }
    }
}