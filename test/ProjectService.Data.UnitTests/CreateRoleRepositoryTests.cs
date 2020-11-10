using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

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
            role.Name = "Name";
            role.Id = Guid.NewGuid();

            Assert.AreEqual(role.Id, repository.CreateRole(role));
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