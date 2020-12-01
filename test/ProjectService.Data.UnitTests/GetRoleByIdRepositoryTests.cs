using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
{
    class GetRoleByIdRepositoryTests
    {
        private IDataProvider provider;
        private IRoleRepository repository;

        private DbRole dbRole;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            dbRole = new DbRole
            {
                Id = Guid.NewGuid(),
                Name = "Role",
                Description = "Role description"
            };
        }

        [SetUp]
        public void SetUp()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                                    .UseInMemoryDatabase("ProjectServiceTest")
                                    .Options;

            provider = new ProjectServiceDbContext(dbOptions);
            repository = new RoleRepository(provider);           

            provider.Roles.Add(dbRole);
            provider.Save();
        }

        [TearDown]
        public void CleanDbMemory()
        {
            if (provider.IsInMemory())
            {
                provider.EnsureDeleted();
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenRoleDoesNotExist()
        {
            var nonexistentId = Guid.NewGuid();

            Assert.That(() => repository.GetRole(nonexistentId),
                Throws.InstanceOf<NotFoundException>().And
                .Message.EqualTo($"Role with id: '{nonexistentId}' was not found."));
            Assert.That(provider.Roles, Is.EquivalentTo(new List<DbRole> { dbRole }));
        }

        [Test]
        public void ShouldReturnRoleInfo()
        {
            var result = repository.GetRole(dbRole.Id);

            var expected = new DbRole
            {
                Id = dbRole.Id,
                Name = dbRole.Name,
                Description = dbRole.Description
            };

            SerializerAssert.AreEqual(expected, result);
            Assert.That(provider.Roles, Is.EquivalentTo(new List<DbRole> { dbRole }));
        }
    }
}
