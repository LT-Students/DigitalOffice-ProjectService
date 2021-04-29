using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    internal class CreateTaskCommandTests
    {
        private Guid _authorId;
        private AutoMocker _mocker;
        private DbTask _dbTask;
        private CreateTaskRequest _newRequest;
        private ICreateNewTaskCommand _command;
        private OperationResultResponse<Guid> _response;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _authorId = Guid.NewGuid();

            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<CreateTaskCommand>();

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _authorId);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _newRequest = new CreateTaskRequest
            {
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = Guid.NewGuid(),
                AuthorId = _authorId,
                Deadline = DateTime.UtcNow,
                ProjectId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ParentTaskId = Guid.NewGuid(),
                Number = 2,
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            };

            _dbTask = new DbTask
            {
                Id = Guid.NewGuid(),
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = _newRequest.AssignedTo,
                AuthorId = _newRequest.AuthorId,
                Deadline = _newRequest.Deadline,
                ProjectId = _newRequest.ProjectId,
                CreatedAt = _newRequest.CreatedAt,
                ParentTaskId = _newRequest.ParentTaskId,
                Number = 2,
                Priority = new DbTaskProperty()
                {
                    Id = _newRequest.PriorityId
                },
                Status = new DbTaskProperty()
                {
                    Id = _newRequest.StatusId
                },
                Type = new DbTaskProperty()
                {
                    Id = _newRequest.TypeId
                }
            };

            _response = new OperationResultResponse<Guid>
            {
                Body = _dbTask.Id
            };
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<ICreateTaskValidator>().Reset();
            _mocker.GetMock<IAccessValidator>().Reset();
            _mocker.GetMock<IDbTaskMapper>().Reset();
            _mocker.GetMock<ITaskRepository>().Reset();
        }

        [Test]
        public void ShouldThrowExceptionWhenCreatingNewTaskWithIncorrectTasktData()
        {
            _mocker
                .Setup<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_newRequest));
            _mocker.Verify<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
        }

        [Test]
        public void ShouldReturnResponseWhenCreatingNewTaskAndUserHasRights()
        {
            _mocker
                .Setup<ICreateTaskValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId))
                .Returns(_dbTask);

            _mocker
                .Setup<ITaskRepository, Guid>(x => x.CreateTask(_dbTask))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
        }

        [Test]
        public void ShouldReturnResponseWhenCreatingNewTaskAndUserIsAdmin()
        {
            _mocker
               .Setup<IAccessValidator, bool>(x => x.IsAdmin())
               .Returns(true);

            _mocker
                .Setup<IDbTaskMapper, DbTask>(x => x.Map(_newRequest, _authorId))
                .Returns(_dbTask);

            _mocker
                .Setup<ITaskRepository, Guid>(x => x.CreateTask(_dbTask))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
        }
    }
}
