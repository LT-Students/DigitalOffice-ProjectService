using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class GetRoleByIdCommandTests
    {
        private IGetRoleCommand _command;

        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IRoleExpandedResponseMapper> _mapperMock;

        private Guid _roleIdRequest;
        private DbRole _dbRole;
        private RoleExpandedResponse _roleResponse;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _roleIdRequest = Guid.NewGuid();

            _dbRole = new DbRole
            {
                Id = _roleIdRequest,
                Name = "Role name test",
                Description = "Role description test",
                IsActive = true,
                Users = new List<DbProjectUser>()
            };

            _roleResponse = new RoleExpandedResponse
            {
                Role = new Role
                {
                    Id = _roleIdRequest,
                    Name = "Role name test",
                    Description = "Role description test"
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IRoleExpandedResponseMapper>();
            _command = new GetRoleCommand(_roleRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            _roleRepositoryMock
                .Setup(r => r.GetRole(It.IsAny<Guid>()))
                .Throws(new NotFoundException());

            _mapperMock
                .Setup(m => m.Map(It.IsAny<DbRole>()))
                .Returns(_roleResponse);

            Assert.Throws<NotFoundException>(() => _command.Execute(Guid.NewGuid()));
            _roleRepositoryMock.Verify(repository => repository.GetRole(It.IsAny<Guid>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<DbRole>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsIt()
        {
            _roleRepositoryMock
                .Setup(r => r.GetRole(It.IsAny<Guid>()))
                .Returns(_dbRole);

            _mapperMock
                .Setup(m => m.Map(It.IsAny<DbRole>()))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _command.Execute(Guid.NewGuid()));
            _roleRepositoryMock.Verify(repository => repository.GetRole(It.IsAny<Guid>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<DbRole>()), Times.Once);
        }

        [Test]
        public void ShouldReturnRoleInfo()
        {
            var expectedRole = new Role
            {
                Id = _roleIdRequest,
                Name = "Role name test",
                Description = "Role description test"
            };

            _roleRepositoryMock
                .Setup(r => r.GetRole(It.IsAny<Guid>()))
                .Returns(_dbRole);

            _mapperMock
                .Setup(m => m.Map(It.IsAny<DbRole>()))
                .Returns(_roleResponse);

            var result = _command.Execute(_roleIdRequest);

            SerializerAssert.AreEqual(expectedRole, result.Role);
        }
    }
}
