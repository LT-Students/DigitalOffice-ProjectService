using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    class CreateTaskPropertyValidatorTests
    {
        private CreateTaskPropertyRequest _request;
        private Mock<ITaskPropertyRepository> _repository;
        private ICreateTaskPropertyValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repository = new Mock<ITaskPropertyRepository>();

            _validator = new CreateTaskPropertyValidator(_repository.Object);

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

        [SetUp]
        public void SetUp()
        {
            _repository.Reset();

            _repository
                .Setup(x => x.AreExistForProject(_request.ProjectId, It.IsAny<string[]>()))
                .Returns(false);
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
            _repository.Verify(x => x.AreExistForProject(_request.ProjectId, It.IsAny<string[]>()), Times.Never);
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
            _repository.Verify(x => x.AreExistForProject(_request.ProjectId, It.IsAny<string[]>()), Times.Never);
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
            _repository.Verify(x => x.AreExistForProject(_request.ProjectId, It.IsAny<string[]>()), Times.Never);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenPropertyIsAreadyExist()
        {
            _repository
                .Setup(x => x.AreExistForProject(_request.ProjectId, It.IsAny<string[]>()))
                .Returns(true);

            _validator.TestValidate(_request).ShouldHaveAnyValidationError();
            _repository.Verify(x => x.AreExistForProject(_request.ProjectId, It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public void ShouldValidRequest()
        {
            _validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
            _repository.Verify(x => x.AreExistForProject(_request.ProjectId, It.IsAny<string[]>()), Times.Once);
        }
    }
}
