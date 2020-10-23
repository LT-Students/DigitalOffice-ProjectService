using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    class CreateRoleRepositoryTests
    {
        private IDataProvider provider;
        private IRoleRepository repository;

        private DbRole role;

        [SetUp]
        public void SetUp()
        {
            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("RoleTest")
                .Options;

            provider = new ProjectServiceDbContext(dbOptionsProjectService);

            repository = new RoleRepository(provider);

            role = new DbRole
            {
                Id = Guid.NewGuid(),
                Name = "Tester",
                Description = "Engaged in testing"
            };

            repository.CreateRole(role);
        }

        [Test]
        public void ShouldAddRoleSuccessfully()
        {
            var roleId = Guid.NewGuid();

            var dbRole = new DbRole();
            dbRole.Id = roleId;
            dbRole.Name = "Name";

            Assert.AreEqual(roleId, repository.CreateRole(dbRole));
        }

        [Test]
        public void ShouldThrowExceptionWhenDbRoleIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => repository.CreateRole(null));
        }

        [TearDown]
        public void CleanMemoryDb()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }
    }
}