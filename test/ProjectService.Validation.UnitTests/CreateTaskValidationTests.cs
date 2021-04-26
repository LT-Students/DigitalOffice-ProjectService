using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    internal class CreateTaskValidationTests
    {
        private ICreateTaskValidator _validator;
        private CreateTaskRequest _taskRequest;

        [SetUp]
        public void SetUp()
        {
            _validator = new CreateTaskRequestValidator();

            _taskRequest = new CreateTaskRequest
            {
                Name = "Create Task",
                Description = "Do smth after smth",
                Deadline = DateTime.UtcNow
            };
        }
/*
        [Test]
        public void ShouldErrorWhenTaskNameIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Name, "");
        }

        [Test]
        public void ShouldErrorWhenTaskNameIsTooLong()
        {
            var name = "Task Name".PadLeft(150);

            _validator.ShouldHaveValidationErrorFor(x => x.Name, name);
        }

        [Test]
        public void ShouldErrorsWhenDescriptionIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.Description, "");
        }

        [Test]
        public void ShouldErrorWhenDeadlineLessThanCreatedTime()
        {
            var deadline = DateTime.UtcNow;
            var createdAt = DateTime.UtcNow;
            _validator.ShouldHaveValidationErrorFor(x => x.Deadline);
        }

        [Test]
        public void ShouldNotErrorsWhenRequestIsValid()
        {
            _validator.TestValidate(_taskRequest).ShouldNotHaveAnyValidationErrors();
        }*/
    }
}
