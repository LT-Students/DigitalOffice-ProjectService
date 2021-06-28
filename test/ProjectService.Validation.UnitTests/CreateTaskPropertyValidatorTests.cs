using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    class CreateTaskPropertyValidatorTests
    {
        private CreateTaskPropertyRequest _request;
        private ICreateTaskPropertyValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new CreateTaskPropertyValidator();

            var newProperties = new List<TaskProperty>
            {
                new TaskProperty
                {
                    Name = "Bug",
                    PropertyType = TaskPropertyType.Type,
                    Description = "Description"
                },
                new TaskProperty
                {
                    Name = "Feature",
                    PropertyType = TaskPropertyType.Type,
                    Description = "Description"
                }
            };

            _request = new CreateTaskPropertyRequest
            {
                ProjectId = Guid.NewGuid(),
                TaskProperties = newProperties
            };
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdIsEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(x => x.ProjectId, Guid.Empty);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenNameIsEmpty()
        {
            var newProperties = new List<TaskProperty>
            {
                new TaskProperty
                {
                    Name = "",
                    PropertyType = TaskPropertyType.Type,
                    Description = "Description"
                }
            };

            var request = new CreateTaskPropertyRequest
            {
                ProjectId = Guid.NewGuid(),
                TaskProperties = newProperties
            };

            _validator.TestValidate(request).ShouldHaveValidationErrorFor("TaskProperties[0].Name");
        }

        [Test]
        public void ShouldHaveValidationErrorWhenDescriptionIsEmpty()
        {
            var newProperties = new List<TaskProperty>
            {
                new TaskProperty
                {
                    Name = "Name",
                    PropertyType = TaskPropertyType.Type,
                    Description = ""
                }
            };

            var request = new CreateTaskPropertyRequest
            {
                ProjectId = Guid.NewGuid(),
                TaskProperties = newProperties
            };

            _validator.TestValidate(request).ShouldHaveValidationErrorFor("TaskProperties[0].Description");
        }

        [Test]
        public void ShouldHaveValidationErrorWhenPropertyTypeIsNotExist()
        {
            var newProperties = new List<TaskProperty>
            {
                new TaskProperty
                {
                    Name = "Name",
                    PropertyType = (TaskPropertyType)6,
                    Description = "Description"
                }
            };

            var request = new CreateTaskPropertyRequest
            {
                ProjectId = Guid.NewGuid(),
                TaskProperties = newProperties
            };

            _validator.TestValidate(request).ShouldHaveValidationErrorFor("TaskProperties[0].PropertyType");
        }

        [Test]
        public void ShouldValidRequest()
        {
            _validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
        }
    }
}
