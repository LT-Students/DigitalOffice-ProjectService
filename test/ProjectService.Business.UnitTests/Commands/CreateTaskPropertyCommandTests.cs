using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class CreateTaskPropertyCommandTests
    {
        private Guid _projectId;
        private CreateTaskPropertyRequest _request;
        private List<DbTaskProperty> _dbTaskProperties;

        private AutoMocker _mocker;
        private ICreateTaskPropertyCommand _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mocker = new();
            _command = _mocker.CreateInstance<CreateTaskPropertyCommand>();

            _projectId = Guid.NewGuid();
            var authorId = Guid.NewGuid();

            IDictionary<object, object> items = new Dictionary<object, object>();
            items.Add("UserId", authorId);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(items);

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
                ProjectId = _projectId,
                TaskProperties = newProperties
            };

            _dbTaskProperties = new();

            foreach (var dbTaskProperty in _request.TaskProperties)
            {
                _dbTaskProperties.Add(
                    new DbTaskProperty
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = authorId,
                        ProjectId = _projectId,
                        Name = dbTaskProperty.Name,
                        IsActive = true,
                        Description = dbTaskProperty.Description,
                        PropertyType = (int)dbTaskProperty.PropertyType,
                    }
                );
            }
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<IAccessValidator>().Reset();
            _mocker.GetMock<ICreateTaskPropertyValidator>().Reset();
            _mocker.GetMock<ITaskPropertyRepository>().Reset();
            _mocker.GetMock<IDbTaskPropertyMapper>().Reset();
        }

        [Test]
        public void ShouldThrowExceptionWhenUserNotEnoughRights()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(_request));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
            _mocker.Verify<ICreateTaskPropertyValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Never);
            _mocker.Verify<ITaskPropertyRepository>(x => x.Create(_dbTaskProperties), Times.Never);
            _mocker.Verify<IDbTaskPropertyMapper, DbTaskProperty>(x =>
                x.Map(It.IsAny<TaskProperty>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public void ShouldCreateTaskPropertySuccessful()
        {
            var response = new OperationResultResponse<IEnumerable<Guid>>
            {
                Body = _dbTaskProperties.Select(x => x.Id),
                Status = OperationResultStatusType.FullSuccess
            };

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            _mocker
                .Setup<ICreateTaskPropertyValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);


            _mocker
                .SetupSequence<IDbTaskPropertyMapper, DbTaskProperty>(x => x.Map(It.IsAny<TaskProperty>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(_dbTaskProperties[0])
                .Returns(_dbTaskProperties[1]);

            _mocker
                .Setup<ITaskPropertyRepository>(x => x.Create(_dbTaskProperties));

            SerializerAssert.AreEqual(response, _command.Execute(_request));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
            _mocker.Verify<ICreateTaskPropertyValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
            _mocker.Verify<ITaskPropertyRepository>(x => x.Create(_dbTaskProperties), Times.Once);
            _mocker.Verify<IDbTaskPropertyMapper, DbTaskProperty>(x =>
                x.Map(It.IsAny<TaskProperty>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Exactly(_dbTaskProperties.Count));
        }
    }
}
