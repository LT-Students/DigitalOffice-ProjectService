using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands
{
    internal class EditProjectByIdCommandTests
    {
        private AutoMocker _mocker;
        private JsonPatchDocument<EditProjectRequest> _request;
        private JsonPatchDocument<DbProject> _dbRequest;
        private IEditProjectCommand _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _request = new JsonPatchDocument<EditProjectRequest>();
            _dbRequest = new JsonPatchDocument<DbProject>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _mocker
                .Setup<IEditProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin())
                .Returns(true);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            _mocker
                .Setup<IEditProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request))
                .Returns(_dbRequest);

            _mocker
                .Setup<IProjectRepository, bool>(x => x.EditProject(It.IsAny<Guid>(), _dbRequest))
                .Returns(true);

            _command = _mocker.CreateInstance<EditProjectCommand>();
        }

        [Test]
        public void SuccessCommandExecuteWhenAdminandHasRights()
        {
            SerializerAssert.AreEqual(true, _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void SuccessCommandExecuteWhenNotAdmin()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin())
                .Returns(false);

            SerializerAssert.AreEqual(true, _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void SuccessCommandExecuteWhenNotRights()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            SerializerAssert.AreEqual(true, _command.Execute(It.IsAny<Guid>(), _request));
        }


        [Test]
        public void ValidationExceptionWhenInvalidRequest()
        {
            _mocker
                .Setup<IEditProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void ForbiddenExceptionWhenUserIsNotAdminAndHasNoRights()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin())
                .Returns(false);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void ArgumentNullExceptionWhenMapperThrowsIt()
        {
            _mocker
                .Setup<IEditProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void NullReferenceExceptionWhenDbProjectNotFound()
        {
            _mocker
                .Setup<IProjectRepository, bool>(x => x.EditProject(It.IsAny<Guid>(), _dbRequest))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => _command.Execute(It.IsAny<Guid>(), _request));
        }
    }
}