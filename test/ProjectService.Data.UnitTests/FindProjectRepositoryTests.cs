using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    class FindProjectRepositoryTests
    {
        private FindProjectsFilter _filter;
        private IDataProvider _provider;
        private IProjectRepository _repository;

        private List<DbProject> _dbProjectsInDb;
        private DbProject _dbProject1;
        private DbProject _dbProject2;
        private DbProject _dbProject3;
        private DbProject _dbProject4;

        [SetUp]
        public void SetUp()
        {
            _dbProject1 = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Name1",
                ShortName = "N1",
                Description = "description",
                DepartmentId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            };

            _dbProject2 = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Name2",
                ShortName = "N2",
                Description = "description",
                DepartmentId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            };

            _dbProject3 = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "NameWithRegular1",
                ShortName = "NWR1",
                Description = "description",
                DepartmentId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            };

            _dbProject4 = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "NameWithRegular2",
                ShortName = "NWR2",
                Description = "description",
                DepartmentId = _dbProject3.DepartmentId,
                CreatedAt = DateTime.UtcNow,
            };

            _dbProjectsInDb = new List<DbProject>
            {
                _dbProject1,
                _dbProject2,
                _dbProject3,
                _dbProject4
            };

            CreateMemoryDb();

            _provider.Projects.AddRange(_dbProjectsInDb);
            _provider.Save();
        }

        [TearDown]
        public void CleanDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }

        public void CreateMemoryDb()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                   .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                   .Options;
            _provider = new ProjectServiceDbContext(dbOptions);

            _repository = new ProjectRepository(_provider);
        }

        [Test]
        public void ShouldReturnProjectsByDepartmentId()
        {
            var pairs = new Dictionary<Guid, string>();
            pairs.Add(_dbProject3.DepartmentId, "");

            _filter = new FindProjectsFilter
            {
                DepartmentId = _dbProject3.DepartmentId
            };

            var expectedProjects = new List<DbProject>
            {
                _dbProject3,
                _dbProject4
            };

            var result = _repository.FindProjects(_filter, 0, 3, out int totalCount);

            Assert.IsTrue(result.Contains(_dbProject4) && result.Contains(_dbProject3));
            Assert.AreEqual(expectedProjects.Count, totalCount);
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFilterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.FindProjects(null, 0, 0, out int _));
        }

        [Test]
        public void ShouldSearchProject()
        {
            List<DbProject> projects = new()
            {
                _dbProject3,
                _dbProject4
            };

            SerializerAssert.AreEqual(projects, _repository.Search("Regular"));
        }

        [Test]
        public void ShouldThrowNullArgumentExceptionWhenSearchTextIsNullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Search(""));
            Assert.Throws<ArgumentNullException>(() => _repository.Search(null));
        }
    }
}
