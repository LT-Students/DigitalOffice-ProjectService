using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Mappers;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Mappers
{
    public class RoleMapperTests
    {
        private IMapper<DbRole, Role> dbToDtoMapper;
        private IMapper<CreateRoleRequest, DbRole> createRoleRequestToDbMapper;

        private const string NAME = "Role";
        private const string DESCRIPTION = "Role in DigitalOffice. From a student who does a work, sometimes.";

        private Guid roleId;

        private DbRole dbRole;
        private CreateRoleRequest roleRequest;

        [SetUp]
        public void SetUp()
        {
            dbToDtoMapper = new RoleMapper();
            createRoleRequestToDbMapper = new RoleMapper();

            roleId = Guid.NewGuid();

            dbRole = new DbRole
            {
                Id = roleId
            };

            roleRequest = new CreateRoleRequest
            {
                Name = NAME,
                Description = DESCRIPTION
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbRoleIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => dbToDtoMapper.Map(null));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenCreateRoleRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => createRoleRequestToDbMapper.Map(null));
        }

        [Test]
        public void ShouldReturnRoleModelWhenDbRoleIsMapped()
        {
            var result = dbToDtoMapper.Map(dbRole);

            var expected = new Role
            {
                Name = dbRole.Name,
                Description = dbRole.Description
            };

            SerializerAssert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnDbRoleModelWhenCreateRoleRequestIsMapped()
        {
            var role = createRoleRequestToDbMapper.Map(roleRequest);

            var expectedDbRole = new DbRole
            {
                Id = role.Id,
                Name = roleRequest.Name,
                Description = roleRequest.Description
            };

            Assert.IsInstanceOf<Guid>(role.Id);
            SerializerAssert.AreEqual(expectedDbRole, role);
        }
    }
}