using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    internal class CreateNewProjectRepositoryTests
    {
        private IDataProvider _provider;
        private IProjectRepository _repository;
        private Mock<IHttpContextAccessor> _accessorMock;

        private DbProject _newProject;
        private Guid _creatorId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _accessorMock = new();
            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _creatorId);

            _accessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(_items);

            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("ProjectServiceTest")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptionsProjectService);

            _repository = new ProjectRepository(_provider, _accessorMock.Object);

            _newProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                DepartmentId = Guid.NewGuid(),
                Status = (int)ProjectStatusType.Active,
                CreatedBy = _creatorId,
                Users = new List<DbProjectUser>
                {
                    new DbProjectUser
                    {
                        Id = Guid.NewGuid(),
                        Role = (int)ProjectUserRoleType.Manager,
                        CreatedBy = _creatorId
                    }
                }
            };

            _provider.Projects.Add(_newProject);
            _provider.Save();
        }

        [TearDown]
        public void CleanMemoryDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }

        [Test]
        public void ShouldAddNewProjectWhenTheNameWasRepeated()
        {
            var dbProject = _provider.Projects.FirstOrDefault(project => project.Id == _newProject.Id);
            dbProject.Users.ElementAt(0).Project = null;
            Assert.IsTrue(_repository.IsExist(_newProject.Id));
            SerializerAssert.AreEqual(_newProject, dbProject);
        }
    }
}