using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    internal class AddUsersToProjectCommandTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private Mock<IProjectUserRequestMapper> _mapperMock;
        private Mock<IAccessValidator> _accessValidatorMock;
        private Mock<IAddUsersToProjectValidator> _validatorMock;

        private AddUsersToProjectCommand _command;
        private AddUsersToProjectRequest _request;

        private List<DbProjectUser> _dbProjectUsers;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var projectUsers = new List<ProjectUserRequest>
            {
                new ProjectUserRequest
                {
                    Role = UserRoleType.ProjectAdmin,
                    UserId = Guid.NewGuid()
                },
                new ProjectUserRequest
                {
                    Role = UserRoleType.ProjectAdmin,
                    UserId = Guid.NewGuid()
                }
            };

            _request = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = projectUsers
            };

            _dbProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _request.ProjectId,
                    UserId = projectUsers.ElementAt(0).UserId,
                    AddedOn = DateTime.Now,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _request.ProjectId,
                    UserId = projectUsers.ElementAt(1).UserId,
                    AddedOn = DateTime.Now,
                    IsActive = true
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IProjectUserRequestMapper>();
            _accessValidatorMock = new Mock<IAccessValidator>();
            _validatorMock = new Mock<IAddUsersToProjectValidator>();

            _command = new AddUsersToProjectCommand(
                _repositoryMock.Object,
                _mapperMock.Object,
                _accessValidatorMock.Object,
                _validatorMock.Object);
        }

        [Test]
        public void ShouldForbiddenExceptionWhenUserIsNotAdmin()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(false)
                .Verifiable();

            Assert.Throws<ForbiddenException>(() => _command.Execute(_request));
            _accessValidatorMock.Verify();
        }

        [Test]
        public void ShouldForbiddenExceptionWhenuserHasEnoughRights()
        {
            int accessRightId = 2;

            _accessValidatorMock
                .Setup(x => x.HasRights(accessRightId))
                .Returns(false)
                .Verifiable();

            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(false)
                .Verifiable();

            Assert.Throws<ForbiddenException>(() => _command.Execute(_request));
            _accessValidatorMock.Verify();
        }

        [Test]
        public void ShouldValidationExeptionWhenRequestDataIsNotValid()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(true)
                .Verifiable();

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false)
                .Verifiable();

            Assert.Throws<ValidationException>(() => _command.Execute(_request));
            _accessValidatorMock.Verify();
            _validatorMock.Verify();
        }

        [Test]
        public void ShouldArgumentNullExceptionWhenProjectUserRequestIsNull()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(true)
                .Verifiable();

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true)
                .Verifiable();

            _mapperMock
                .Setup(x => x.Map(It.IsAny<ProjectUserRequest>()))
                .Throws(new ArgumentNullException())
                .Verifiable();

            Assert.Throws<ArgumentNullException>(() => _command.Execute(_request));
            _accessValidatorMock.Verify();
            _validatorMock.Verify();
            _mapperMock.Verify();
        }

        [Test]
        public void ShouldArgumentNullExceptionWhenDbProjectUserIsNull()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(true)
                .Verifiable();

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true)
                .Verifiable();

            _mapperMock
                .SetupSequence(x => x.Map(It.IsAny<ProjectUserRequest>()))
                .Returns(_dbProjectUsers.ElementAt(0))
                .Returns(_dbProjectUsers.ElementAt(1));

            _repositoryMock
                .Setup(x => x.AddUsersToProject(_dbProjectUsers, _request.ProjectId))
                .Throws(new ArgumentNullException())
                .Verifiable();

            Assert.Throws<ArgumentNullException>(() => _command.Execute(_request));
            _mapperMock.Verify(x => x.Map(It.IsAny<ProjectUserRequest>()), Times.Exactly(2));
            _accessValidatorMock.Verify();
            _repositoryMock.Verify();
        }

        [Test]
        public void ShouldArgumentNullExceptionWhenProjectIdIsNotExist()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(true)
                .Verifiable();

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true)
                .Verifiable();

            _mapperMock
                .SetupSequence(x => x.Map(It.IsAny<ProjectUserRequest>()))
                .Returns(_dbProjectUsers.ElementAt(0))
                .Returns(_dbProjectUsers.ElementAt(1));

            _repositoryMock
                .Setup(x => x.AddUsersToProject(_dbProjectUsers, _request.ProjectId))
                .Throws(new BadRequestException())
                .Verifiable();

            Assert.Throws<BadRequestException>(() => _command.Execute(_request));
            _mapperMock.Verify(x => x.Map(It.IsAny<ProjectUserRequest>()), Times.Exactly(2));
            _accessValidatorMock.Verify();
            _repositoryMock.Verify();
        }

        [Test]
        public void ShouldAddUsersToProjectSuccessful()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(true)
                .Verifiable();

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true)
                .Verifiable();

            _mapperMock
                .SetupSequence(x => x.Map(It.IsAny<ProjectUserRequest>()))
                .Returns(_dbProjectUsers.ElementAt(0))
                .Returns(_dbProjectUsers.ElementAt(1));

            _repositoryMock
                .Setup(x => x.AddUsersToProject(_dbProjectUsers, _request.ProjectId))
                .Verifiable();

            _command.Execute(_request);

            _accessValidatorMock.Verify();
            _validatorMock.Verify();
            _repositoryMock.Verify();
            _mapperMock.Verify(x => x.Map(It.IsAny<ProjectUserRequest>()), Times.Exactly(2));
        }
    }
}
