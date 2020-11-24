﻿using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    class DeleteRoleRepositoryTests
    {
        private IDataProvider provider;
        private IRoleRepository repository;

        private DbRole dbRole;

        private void CreateMemoryContext()
        {
            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("RoleTest")
                .Options;

            provider = new ProjectServiceDbContext(dbOptionsProjectService);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            dbRole = new DbRole
            {
                Id = Guid.NewGuid(),
                Name = "New role",
                Description = "New role description"
            };
        }

        [SetUp]
        public void SetUp()
        {
            CreateMemoryContext();

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
        public void ShouldDeleteRoleSuccessfully()
        {
            var deleteId = dbRole.Id;

            Assert.IsTrue(repository.DeleteRole(deleteId));
        }

        [Test]
        public void ShouldThrowNullReferenceExceptionWhenRoleIdNotFound()
        {
            var deleteId = Guid.NewGuid();

            Assert.That(() => repository.DeleteRole(deleteId),
                Throws.InstanceOf<NullReferenceException>().And
                .Message.EqualTo("Role with this Id does not exist."));
        }
    }
}