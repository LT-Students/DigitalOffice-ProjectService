using FluentValidation.TestHelper;
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

        private CreateTaskRequest _taskRequest;

        [SetUp]
        public void SetUp()
        {
            _taskRepository = new Mock<ITaskRepository>();
            _userRepository = new Mock<IUserRepository>();
            _projectRepository = new Mock<IProjectRepository>();

            _validator = new CreateTaskRequestValidator(_taskRepository.Object, _userRepository.Object, _projectRepository.Object);

            _taskRequest = new CreateTaskRequest
            {
                Name = "Create Task",
                Description = "Do smth after smth",
                Deadline = DateTime.UtcNow,
                ProjectId = Guid.NewGuid(),
                AssignedTo = Guid.NewGuid(),
                ParentTaskId = Guid.NewGuid()
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
        public void ShouldThrowErrorWhenDeadlineLessThanCreatedTime()
        {
            _validator.TestValidate(new CreateTaskRequest
            {
                Deadline = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow.AddDays(1)
            }).ShouldHaveValidationErrorFor(x => x.Deadline);
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
                .Setup(x => x.IsExist(_taskRequest.ParentTaskId.Value))
                .Returns(false)
                .Verifiable();

            _validator.ShouldHaveValidationErrorFor(x => x.ParentTaskId, _taskRequest.ParentTaskId.Value);
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
                .Setup(x => x.IsExist(_taskRequest.ParentTaskId.Value))
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
        public void ShouldNotErrorsWhenRequestIsValid()
        {
            _userRepository
                .Setup(x => x.AreUserAndProjectExist(_taskRequest.AssignedTo.Value, _taskRequest.ProjectId))
                .Returns(true)
                .Verifiable();

            _taskRepository
                .Setup(x => x.IsExist(_taskRequest.ParentTaskId.Value))
                .Returns(true)
                .Verifiable();

            _projectRepository
               .Setup(x => x.IsExist(_taskRequest.ProjectId))
               .Returns(true)
               .Verifiable();

            _validator.TestValidate(_taskRequest).ShouldNotHaveAnyValidationErrors();
        }
    }
}
