using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Mappers
{
    class RoleMapperTests
    {
        private const string Name = "Test Role";
        private const string Description = "Test Role description";

        private IRoleMapper roleMapper;
        private IRoleExpandedResponseMapper roleResponseMapper;
        private IRolesResponseMapper rolesResponseMapper;

        private Guid roleId;

        private DbRole dbRole;

        [SetUp]
        public void SetUp()
        {
            roleMapper = new RoleMapper();
            roleResponseMapper = new RoleExpandedResponseMapper(roleMapper);
            rolesResponseMapper = new RolesResponseMapper(roleMapper);

            roleId = Guid.NewGuid();

            dbRole = new DbRole
            {
                Id = roleId,
                Name = Name,
                Description = Description
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => roleResponseMapper.Map(null));
            Assert.Throws<ArgumentNullException>(() => rolesResponseMapper.Map(null));
        }

        public void ShouldReturnRoleExpandedResponseModelWhenDbRoleIsMapped()
        {
            var result = roleResponseMapper.Map(dbRole);

            var expected = new RoleExpandedResponse
            {
                Role = roleMapper.Map(dbRole)
            };

            SerializerAssert.AreEqual(expected, result);
        }

        public void ShouldReturnRolesResponseModelWhenDbRoleIsMapped()
        {
            var result = roleResponseMapper.Map(dbRole);

            var expected = new RolesResponse
            {
                Roles = roleMapper.Map(dbRole)
            };

            SerializerAssert.AreEqual(expected, result);
        }
    }
}
