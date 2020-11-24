using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests.Commands
{
    class CreateRoleCommandTests
    {
        private ICreateRoleCommand command;

        private Mock<IRoleRepository> repositoryMock;
        private Mock<IValidator<CreateRoleRequest>> validatorMock;
        private Mock<ICreateRoleRequestMapper> mapperMock;

        private DbRole role;

        private CreateRoleRequest roleRequest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            roleRequest = new CreateRoleRequest
            {
                Name = "Tester",
                Description = "Engaged in testing"
            };

            role = new DbRole
            {
                Id = Guid.NewGuid(),
                Name = roleRequest.Name,
                Description = roleRequest.Description
            };
        }

        [SetUp]
        public void SetUp()
        {
            validatorMock = new Mock<IValidator<CreateRoleRequest>>();
            repositoryMock = new Mock<IRoleRepository>();
            mapperMock = new Mock<ICreateRoleRequestMapper>();

            command = new CreateRoleCommand(repositoryMock.Object, validatorMock.Object, mapperMock.Object);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenCreatingRoleWithIncorrectRoleData()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(roleRequest), "Role field validation error");
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperMock.Verify(mapper => mapper.Map(It.IsAny<CreateRoleRequest>()), Times.Never);
            repositoryMock.Verify(repository => repository.CreateRole(It.IsAny<DbRole>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            mapperMock
                .Setup(x => x.Map(It.IsAny<CreateRoleRequest>()))
                .Returns(role)
                .Verifiable();

            repositoryMock
                .Setup(x => x.CreateRole(It.IsAny<DbRole>()))
                .Throws(new ArgumentNullException())
                .Verifiable();

            Assert.Throws<ArgumentNullException>(() => command.Execute(roleRequest));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify();
            mapperMock.Verify();
        }

        [Test]
        public void ShouldReturnIdWhenCreatingNewRole()
        {
            validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            mapperMock
                .Setup(x => x.Map(It.IsAny<CreateRoleRequest>()))
                .Returns(role)
                .Verifiable();

            repositoryMock
                .Setup(x => x.CreateRole(It.IsAny<DbRole>()))
                .Returns(role.Id)
                .Verifiable();

            Assert.AreEqual(role.Id, command.Execute(roleRequest));
            mapperMock.Verify();
            repositoryMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            mapperMock.Setup(x => x.Map(It.IsAny<CreateRoleRequest>())).Throws(new Exception());
            Assert.Throws<Exception>(() => command.Execute(null));
        }
    }
}
