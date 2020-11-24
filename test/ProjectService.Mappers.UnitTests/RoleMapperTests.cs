using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Mappers
{
    public class RoleMapperTests
    {
        private const string Name = "Test Role";
        private const string Description = "Role in DigitalOffice project. The students do the work. Sometimes. Never (c) Spartak. I would like to give the one who did it a medal (c) Spartak";

        private IRoleMapper mapper;

        private Guid roleId;

        private DbRole dbRole;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            roleId = Guid.NewGuid();

            dbRole = new DbRole
            {
                Id = roleId,
                Name = Name,
                Description = Description
            };
        }

        [SetUp]
        public void SetUp()
        {
            mapper = new RoleMapper();
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbRoleIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mapper.Map(null));
        }

        [Test]
        public void ShouldReturnProjectModelWhenDbProjectIsMapped()
        {
            var result = mapper.Map(dbRole);

            var expected = new Role
            {
                Id = roleId,
                Name = Name,
                Description = Description
            };

            SerializerAssert.AreEqual(expected, result);
        }
    }
}