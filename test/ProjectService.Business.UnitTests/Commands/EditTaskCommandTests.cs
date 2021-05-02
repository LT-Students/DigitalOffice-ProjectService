using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
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
    public class EditTaskCommandTests
    {
        #region Private Models
        
        private IEditTaskCommand _command;

        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IEditTaskValidator> _validatorMock;
        private Mock<IAccessValidator> _accessValidatorMock;
        private Mock<IHttpContextAccessor> _httpAccessorMock;
        private IPatchDbTaskMapper _mapper;
        private Mock<ILogger<EditTaskCommand>> _loggerMock;

        private Mock<IRequestClient<IGetDepartmentRequest>> _requestClient;
        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _operationResultBrokerMock;
        
        private JsonPatchDocument<EditTaskRequest> _request;

        #endregion
        
        #region Private fields

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

        private readonly OperationResultResponse<bool> _fullSuccessModel = new()
        {
            Status = OperationResultStatusType.FullSuccess,
            Body = true,
            Errors = new List<string>()
        };

        #endregion

        private void ClientRequestUp(Guid newGuid)
        {
            IDictionary<object, object> httpContextItems = new Dictionary<object, object>();

            httpContextItems.Add("UserId", newGuid);

            _httpAccessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(httpContextItems);
        }

        private void RcGetDepartment(Guid departmentId)
        {
            var department = new Mock<IGetDepartmentResponse>();
            department.Setup(x => x.Id).Returns(departmentId);
            department.Setup(x => x.Name).Returns("Department name");
            
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
                .Returns(Task.FromResult(_operationResultBrokerMock.Object));
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _request = new JsonPatchDocument<EditTaskRequest>(new List<Operation<EditTaskRequest>>()
            {
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.Name)}",
                    "",
                    _name),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.Description)}",
                    "",
                    _description),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.AssignedTo)}",
                    "",
                    _assignedTo),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.PlannedMinutes)}",
                    "",
                    _plannedMinutes),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.PriorityId)}",
                    "",
                    _priorityId),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.StatusId)}",
                    "",
                    _statusId),
                new Operation<EditTaskRequest>(
                    "replace",
                    $"/{nameof(EditTaskRequest.TypeId)}",
                    "",
                    _typeId),
            }, new CamelCasePropertyNamesContractResolver());
        }

        [SetUp]
        public void SetUp()
        {
            #region Mock initialization

            _taskRepositoryMock = new Mock<ITaskRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _validatorMock = new Mock<IEditTaskValidator>();
            _accessValidatorMock = new Mock<IAccessValidator>();
            _httpAccessorMock = new Mock<IHttpContextAccessor>();
            _mapper = new PatchDbTaskMapper();
            _loggerMock = new Mock<ILogger<EditTaskCommand>>();
            _requestClient = new Mock<IRequestClient<IGetDepartmentRequest>>();
            _operationResultBrokerMock = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
            
            #endregion

            ClientRequestUp(Guid.NewGuid());
            RcGetDepartment(Guid.NewGuid());
            
            #region Mock default setups

            _taskRepositoryMock
                .Setup(x => x.Edit(_taskId, _mapper.Map(_request)))
                .Returns(true);

            _taskRepositoryMock
                .Setup(x => x.Get(_taskId))
                .Returns(new DbTask
                {
                    ProjectId = _projectId
                });
            
            _accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);
            
            _projectRepositoryMock.Setup(x => x.GetProject(_projectId)).Returns(new DbProject()
            {
                Users = new List<DbProjectUser>()
                {
                    new DbProjectUser
                    {
                        UserId = Guid.NewGuid()
                    }
                }
            });

            #endregion
            
            _command = new EditTaskCommand(
                _taskRepositoryMock.Object,
                _projectRepositoryMock.Object,
                _validatorMock.Object,
                _accessValidatorMock.Object,
                _httpAccessorMock.Object,
                _mapper,
                _loggerMock.Object,
                _requestClient.Object);
        }

        [Test]
        public void FullSuccessOperationWhenUserIsAdmin()
        {
            _accessValidatorMock.Setup(x => x.IsAdmin()).Returns(true);

            SerializerAssert.AreEqual(_fullSuccessModel, _command.Execute(_taskId, _request));
        }

        [Test]
        public void FullSuccessOperationWhenUserInProject()
        {
            Guid userId = Guid.NewGuid();

            ClientRequestUp(userId);
            
            _projectRepositoryMock.Setup(x => x.GetProject(_projectId)).Returns(new DbProject()
            {
                Users = new List<DbProjectUser>()
                {
                    new DbProjectUser
                    {
                        UserId = userId
                    }
                }
            });

            SerializerAssert.AreEqual(_fullSuccessModel, _command.Execute(_taskId, _request));
        }

        [Test]
        public void FullSuccessWhenUserIsDepartmentDirector()
        {
            RcGetDepartment(_departmentId);
            
            _projectRepositoryMock.Setup(x => x.GetProject(_projectId)).Returns(new DbProject()
            {
                DepartmentId = _departmentId
            });
            

            SerializerAssert.AreEqual(_fullSuccessModel, _command.Execute(_taskId, _request));
        }

        [Test]
        public void ExceptionWhenUserCannotEdit()
        {
            Assert.Throws<ForbiddenException>(() => _command.Execute(_taskId, _request));
        }

        [Test]
        public void ExceptionWhenRequestClientUnavailable()
        {
            _accessValidatorMock.Setup(x => x.IsAdmin()).Returns(true);
            
            _requestClient
                .Setup(x => 
                    x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                        It.IsAny<object>(), 
                        default, 
                        default))
                .Returns((Task<Response<IOperationResult<IGetDepartmentResponse>>>) null);
            
            Assert.AreEqual(1, _command.Execute(_taskId, _request).Errors.Count);
        }
    }
}
