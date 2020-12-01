using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.ResponseMappers
{
    class RoleExpandedResponseMapperTests
    {
        private const string Name = "Role name test";
        private const string Description = "Role desription test";

        private IRoleExpandedResponseMapper _roleResponseMapper;

        private Guid _roleId;

        private DbRole _dbRole;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var roleMapper = new RoleMapper();
            _roleResponseMapper = new RoleExpandedResponseMapper(roleMapper);

            _roleId = Guid.NewGuid();

            _dbRole = new DbRole
            {
                Id = _roleId,
                Name = Name,
                Description = Description
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _roleResponseMapper.Map(null));
        }

        [Test]
        public void ShouldReturnRoleExpandedResponseModelWhenDbRoleIsMapped()
        {
            var result = _roleResponseMapper.Map(_dbRole);

            var expected = new RoleExpandedResponse
            {
                Role = new Role
                {
                    Id = _roleId,
                    Name = Name,
                    Description = Description
                }
            };

            SerializerAssert.AreEqual(expected, result);
        }
    }
}

