//using FluentValidation;
//using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
//using LT.DigitalOffice.Kernel.Broker;
//using LT.DigitalOffice.Kernel.Enums;
//using LT.DigitalOffice.Kernel.Exceptions.Models;
//using LT.DigitalOffice.Models.Broker.Requests.Company;
//using LT.DigitalOffice.Models.Broker.Responses.Company;
//using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
//using LT.DigitalOffice.ProjectService.Data.Interfaces;
//using LT.DigitalOffice.ProjectService.Data.Provider;
//using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
//using LT.DigitalOffice.ProjectService.Models.Db;
//using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
//using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
//using LT.DigitalOffice.ProjectService.Validation.Interfaces;
//using LT.DigitalOffice.UnitTestKernel;
//using MassTransit;
//using Microsoft.AspNetCore.Http;
//using Moq;
//using Moq.AutoMock;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;

//namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
//{
//    internal class CreateTaskCommandTests
//    {
//        private AutoMocker _mocker;
//        private DbTask _dbTask;
//        private CreateTaskRequest _newRequest;
//        private ICreateTaskCommand _command;
//        private OperationResultResponse<Guid> _response;
//        private readonly Guid _authorId = Guid.NewGuid();
//        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _operationResultBroker;

//        private void ClientRequestUp(Guid newGuid)
//        {
//            IDictionary<object, object> _items = new Dictionary<object, object>();
//            _items.Add("UserId", _authorId);

//            _mocker
//                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
//                .Returns(_items);
//        }

//        private void RcGetDepartment(Guid departmentId)
//        {
//            var department = new Mock<IGetDepartmentResponse>();
//            department.Setup(x => x.DirectorUserId).Returns(_authorId);

//            _mocker.Setup<Response<IOperationResult<IGetDepartmentResponse>>, IGetDepartmentResponse>(x => x.Message.Body).Returns(department.Object);
//            _mocker.Setup<Response<IOperationResult<IGetDepartmentResponse>>, bool>(x => x.Message.IsSuccess).Returns(true);
//            _mocker.Setup<Response<IOperationResult<IGetDepartmentResponse>>, List<string>>(x => x.Message.Errors).Returns(new List<string>());

//            var responseMock = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();

//            responseMock.Setup(x => x.Message.Body).Returns(department.Object);
//            responseMock.Setup(x => x.Message.IsSuccess).Returns(true);
//            responseMock.Setup(x => x.Message.Errors).Returns(new List<string>());

//            _mocker
//               .Setup<IRequestClient<IGetDepartmentRequest>, Response<IOperationResult<IGetDepartmentResponse>>>(
//               x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//                   It.IsAny<object>(), default, default).Result).Returns(responseMock.Object);
//        }

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _mocker = new AutoMocker();
//            _command = _mocker.CreateInstance<CreateTaskCommand>();

//            _newRequest = new CreateTaskRequest
//            {
//                Name = "Create Smth",
//                Description = "Create smth in somewhere",
//                PlannedMinutes = 30,
//                AssignedTo = Guid.NewGuid(),
//                ProjectId = Guid.NewGuid(),
//                ParentId = Guid.NewGuid(),
//                PriorityId = Guid.NewGuid(),
//                StatusId = Guid.NewGuid(),
//                TypeId = Guid.NewGuid()
//            };

//            _dbTask = new DbTask
//            {
//                Id = Guid.NewGuid(),
//                Name = "Create Smth",
//                Description = "Create smth in somewhere",
//                PlannedMinutes = _newRequest.PlannedMinutes,
//                AssignedTo = _newRequest.AssignedTo,
//                CreatedBy = _authorId,
//                ProjectId = _newRequest.ProjectId,
//                CreatedAtUtc = DateTime.UtcNow,
//                ParentId = _newRequest.ParentId,
//                Number = 2,
//                Priority = new DbTaskProperty()
//                {
//                    Id = _newRequest.PriorityId
//                },
//                Status = new DbTaskProperty()
//                {
//                    Id = _newRequest.StatusId
//                },
//                Type = new DbTaskProperty()
//                {
//                    Id = _newRequest.TypeId
//                }
//            };

//            _response = new OperationResultResponse<Guid>
//            {
//                Body = _dbTask.Id,
//                Status = OperationResultStatusType.FullSuccess,
//                Errors = new List<string>()
//            };

//            ClientRequestUp(Guid.NewGuid());
//            RcGetDepartment(Guid.NewGuid());

//            var department = new Mock<IGetDepartmentResponse>();
//            department.Setup(x => x.DirectorUserId).Returns(_authorId);
//            _operationResultBroker = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
//            _operationResultBroker.Setup(x => x.Message.Body).Returns(department.Object);
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            _mocker.GetMock<ICreateTaskValidator>().Reset();
//            _mocker.GetMock<IAccessValidator>().Reset();
//            _mocker.GetMock<IDbTaskMapper>().Reset();
//            _mocker.GetMock<ITaskRepository>().Reset();
//            _mocker.GetMock<IProjectRepository>().Reset();
//            _mocker.GetMock<IRequestClient<IGetDepartmentRequest>>().Reset();
//            _mocker.GetMock<Response<IOperationResult<IGetDepartmentResponse>>>().Reset();
//            _mocker.GetMock<IHttpContextAccessor>().Reset();
//            _mocker.GetMock<IDataProvider>().Reset();
//            ClientRequestUp(Guid.NewGuid());
//            RcGetDepartment(Guid.NewGuid());

//            _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(true);
//            _operationResultBroker.Setup(x => x.Message.Errors).Returns(new List<string>());
//            _mocker
//              .Setup<IRequestClient<IGetDepartmentRequest>, Response<IOperationResult<IGetDepartmentResponse>>>(
//              x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//                  It.IsAny<object>(), default, default).Result).Returns(_operationResultBroker.Object);
//        }

//        [Test]
//        public void ShouldThrowExceptionWhenCreatingNewTaskWithIncorrectTasktData()
//        {
//            _mocker
//              .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
//              .Returns(true);

//            _mocker
//                .Setup<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                .Returns(false);

//            Assert.Throws<ValidationException>(() => _command.Execute(_newRequest));

//            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
//            _mocker.Verify<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//        }

//        [Test]
//        public void ShouldReturnResponseWhenCreatingNewTaskAndUserIsInProject()
//        {
//            ClientRequestUp(_authorId);

//            _mocker
//              .Setup<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//              .Returns(true);

//            _mocker
//                .Setup<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId))
//                .Returns(_dbTask);

//            _mocker
//                .Setup<ITaskRepository, Guid>(x => x.Create(_dbTask))
//                .Returns(_response.Body);

//            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));

//            _mocker.Verify<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//            _mocker.Verify<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId), Times.Once);
//            _mocker.Verify<ITaskRepository, Guid>(x => x.Create(_dbTask), Times.Once);
//        }

//        [Test]
//        public void ShouldReturnResponseWhenCreatingNewTaskAndUserIsDepartmentDirector()
//        {
//            ClientRequestUp(_authorId);

//            _mocker
//             .Setup<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//             .Returns(true);

//            _mocker
//                .Setup<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId))
//                .Returns(_dbTask);

//            _mocker
//                .Setup<ITaskRepository, Guid>(x => x.Create(_dbTask))
//                .Returns(_response.Body);

//            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));

//            _mocker.Verify<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//            _mocker.Verify<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId), Times.Once);
//            _mocker.Verify<ITaskRepository, Guid>(x => x.Create(_dbTask), Times.Once);
//        }

//        //[Test]
//        //public void ShouldThrowExeptionWhenRequestClientUnavalible()
//        //{
//        //    var newResponse = new OperationResultResponse<Guid>
//        //    {
//        //        Status = OperationResultStatusType.PartialSuccess,
//        //        Errors = new List<string>() { "Cannot create task. Please try again later." }
//        //    };

//        //    _mocker
//        //        .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
//        //        .Returns(false);

//        //    _mocker
//        //    .Setup<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//        //    .Returns(true);

//        //    _mocker
//        //        .Setup<IRequestClient<IGetDepartmentRequest>, Response<IOperationResult<IGetDepartmentResponse>>>(
//        //        x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//        //            It.IsAny<object>(), default, default).Result).Throws(new Exception());

//        //    _mocker
//        //        .Setup<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId))
//        //        .Returns(_dbTask);

//        //    _mocker
//        //        .Setup<ITaskRepository, Guid>(x => x.CreateTask(_dbTask))
//        //        .Returns(null);

//        //    SerializerAssert.AreEqual(newResponse, _command.Execute(_newRequest));

//        //    _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
//        //    _mocker.Verify<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId), Times.Once);
//        //    _mocker.Verify<ITaskRepository, Guid>(x => x.CreateTask(null), Times.Never);
//        //    _mocker.Verify<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//        //    _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Response<IOperationResult<IGetDepartmentResponse>>>(
//        //       x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//        //           It.IsAny<object>(), default, default).Result,
//        //            Times.Once);
//        //}

//        [Test]
//        public void ShouldThrowExceptionWhenUserCannotBeCreated()
//        {
//            _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(false);
//            _operationResultBroker.Setup(x => x.Message.Errors).Returns(new List<string>() { "Department was not found" });

//            Assert.Throws<ForbiddenException>(() => _command.Execute(_newRequest));
//        }

//        [Test]
//        public void ShouldReturnResponseWhenCreatingNewTaskAndUserIsAdmin()
//        {
//            var project = new List<DbProject>
//            {
//                new DbProject { Id = _newRequest.ProjectId}
//            };

//            var task = new List<DbTask>
//            {
//                new DbTask {Id = Guid.NewGuid(), ProjectId = _newRequest.ProjectId, Number = _dbTask.Number}
//            };

//            _mocker
//                  .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
//                  .Returns(true);

//            _mocker
//                 .Setup<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                 .Returns(true);

//            _mocker
//                .Setup<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId))
//                .Returns(_dbTask);

//            _mocker
//                .Setup<ITaskRepository, Guid>(x => x.Create(_dbTask))
//                .Returns(_dbTask.Id);

//            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));

//            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
//            _mocker.Verify<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId), Times.Once);
//            _mocker.Verify<ITaskRepository, Guid>(x => x.Create(_dbTask), Times.Once);
//            _mocker.Verify<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//        }
//    }
//}
