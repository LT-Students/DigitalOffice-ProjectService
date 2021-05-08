﻿using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    internal class CreateTaskValidationTests
    {
        private ICreateTaskValidator _validator;
        private Mock<ITaskRepository> _taskRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IProjectRepository> _projectRepository;
        private Mock<ITaskPropertyRepository> _taskPropertyRepository;
        private CreateTaskRequest _taskRequest;

        [SetUp]
        public void SetUp()
        {
            _taskRepository = new Mock<ITaskRepository>();
            _userRepository = new Mock<IUserRepository>();
            _projectRepository = new Mock<IProjectRepository>();
            _taskPropertyRepository = new Mock<ITaskPropertyRepository>();

            _validator = new CreateTaskRequestValidator(_taskRepository.Object, _userRepository.Object, _projectRepository.Object, _taskPropertyRepository.Object);

            _taskRequest = new CreateTaskRequest
            {
                Name = "Create Task",
                Description = "Do smth after smth",
                ProjectId = Guid.NewGuid(),
                AssignedTo = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            };
        }

        [Test]
        public void ShouldThrowErrorWhenTaskNameIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldThrowErrorWhenTaskNameIsTooLong()
        {
            var name = "Task Name".PadLeft(160);

            _validator.ShouldHaveValidationErrorFor(x => x.Name, name);
        }

        [Test]
        public void ShouldErrorsWhenDescriptionIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Description, "");
        }

        [Test]
        public void ShouldThrowErrorWhenProjectIdIsNull()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.ProjectId, Guid.Empty);
        }

        [Test]
        public void ShouldTrowErrorWhenProjectIdDoesNotExist()
        {
            _projectRepository
                .Setup(x => x.IsExist(_taskRequest.ProjectId))
                .Returns(false)
                .Verifiable();

            _validator.ShouldHaveValidationErrorFor(x => x.ProjectId, _taskRequest.ProjectId);
            _projectRepository.Verify();
        }

        [Test]
        public void ShouldThrowErrorWhenPerentTaskIdDoesNotExist()
        {
            _taskRepository
                .Setup(x => x.IsExist(_taskRequest.ParentId.Value))
                .Returns(false)
                .Verifiable();

            _validator.ShouldHaveValidationErrorFor(x => x.ParentId, _taskRequest.ParentId.Value);
            _taskRepository.Verify();
        }

        [Test]
        public void ShouldThrowErrorWhenAssignedToIdDoesNotExist()
        {
            _userRepository
                .Setup(x => x.AreUserAndProjectExist(_taskRequest.AssignedTo.Value, _taskRequest.ProjectId))
                .Returns(false)
                .Verifiable();

            _taskRepository
                .Setup(x => x.IsExist(_taskRequest.ParentId.Value))
                .Returns(true)
                .Verifiable();

            _projectRepository
               .Setup(x => x.IsExist(_taskRequest.ProjectId))
               .Returns(true)
               .Verifiable();

            _validator.TestValidate(_taskRequest).ShouldHaveAnyValidationError();
            _userRepository.Verify();
        }

        [Test]
        public void ShouldThrowErrorWhenPriorityIdIdDoesNotExist()
        {
            _userRepository
                .Setup(x => x.AreUserAndProjectExist(_taskRequest.AssignedTo.Value, _taskRequest.ProjectId))
                .Returns(false)
                .Verifiable();

            _taskRepository
                .Setup(x => x.IsExist(_taskRequest.ParentId.Value))
                .Returns(true)
                .Verifiable();

            _projectRepository
               .Setup(x => x.IsExist(_taskRequest.ProjectId))
               .Returns(true)
               .Verifiable();

            _taskPropertyRepository
             .Setup(x => x.AreExist(_taskRequest.StatusId))
             .Returns(true)
             .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.PriorityId))
              .Returns(false)
              .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.TypeId))
              .Returns(true)
              .Verifiable();

            _validator.TestValidate(_taskRequest).ShouldHaveAnyValidationError();
            _userRepository.Verify();
        }

        [Test]
        public void ShouldThrowErrorWhenStatusIdIdDoesNotExist()
        {
            _userRepository
                .Setup(x => x.AreUserAndProjectExist(_taskRequest.AssignedTo.Value, _taskRequest.ProjectId))
                .Returns(false)
                .Verifiable();

            _taskRepository
                .Setup(x => x.IsExist(_taskRequest.ParentId.Value))
                .Returns(true)
                .Verifiable();

            _projectRepository
               .Setup(x => x.IsExist(_taskRequest.ProjectId))
               .Returns(true)
               .Verifiable();

            _taskPropertyRepository
             .Setup(x => x.AreExist(_taskRequest.StatusId))
             .Returns(false)
             .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.PriorityId))
              .Returns(true)
              .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.TypeId))
              .Returns(true)
              .Verifiable();

            _validator.TestValidate(_taskRequest).ShouldHaveAnyValidationError();
            _userRepository.Verify();
        }

        [Test]
        public void ShouldThrowErrorWhenTypeIdIdDoesNotExist()
        {
            _userRepository
                .Setup(x => x.AreUserAndProjectExist(_taskRequest.AssignedTo.Value, _taskRequest.ProjectId))
                .Returns(false)
                .Verifiable();

            _taskRepository
                .Setup(x => x.IsExist(_taskRequest.ParentId.Value))
                .Returns(true)
                .Verifiable();

            _projectRepository
               .Setup(x => x.IsExist(_taskRequest.ProjectId))
               .Returns(true)
               .Verifiable();

            _taskPropertyRepository
             .Setup(x => x.AreExist(_taskRequest.StatusId))
             .Returns(true)
             .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.PriorityId))
              .Returns(true)
              .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.TypeId))
              .Returns(false)
              .Verifiable();

            _validator.TestValidate(_taskRequest).ShouldHaveAnyValidationError();
            _userRepository.Verify();
        }

        [Test]
        public void ShouldNotErrorsWhenRequestIsValid()
        {
            _userRepository
                .Setup(x => x.AreUserAndProjectExist(_taskRequest.AssignedTo.Value, _taskRequest.ProjectId))
                .Returns(true)
                .Verifiable();

            _taskRepository
                .Setup(x => x.IsExist(_taskRequest.ParentId.Value))
                .Returns(true)
                .Verifiable();

            _projectRepository
               .Setup(x => x.IsExist(_taskRequest.ProjectId))
               .Returns(true)
               .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.StatusId))
              .Returns(true)
              .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.PriorityId))
              .Returns(true)
              .Verifiable();

            _taskPropertyRepository
              .Setup(x => x.AreExist(_taskRequest.TypeId))
              .Returns(true)
              .Verifiable();

            _validator.TestValidate(_taskRequest).ShouldNotHaveAnyValidationErrors();
        }
    }
}
