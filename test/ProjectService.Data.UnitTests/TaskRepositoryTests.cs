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
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    public class TaskRepositoryTests
    {
        private List<DbTask> _dbTasks;

        private Guid _assign;
        private Guid _projectId;
        private IEnumerable<Guid> _projectIds;
        private IDataProvider _provider;
        private ITaskRepository _repository;
        private DbContextOptions<ProjectServiceDbContext> _dbContextOptions;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjectServiceTest")
                .Options;
            _dbTasks = new List<DbTask>();
            _assign = Guid.NewGuid();
            _projectIds = new List<Guid>();
            _projectId = Guid.NewGuid();

            for (int i = 0; i <= 3; i++)
            {
                _dbTasks.Add(
                    new DbTask
                    {
                        Id = Guid.NewGuid(),
                        Name = "Create Smth",
                        Description = "Create smth in somewhere",
                        PlannedMinutes = 30,
                        AssignedTo = _assign,
                        AuthorId = Guid.NewGuid(),
                        ProjectId = _projectId,
                        CreatedAt = DateTime.UtcNow,
                        ParentId = Guid.NewGuid(),
                        Number = 2,
                        PriorityId = Guid.NewGuid(),
                        StatusId = Guid.NewGuid(),
                        TypeId = Guid.NewGuid()
                    });
            }

            _dbTasks[3].Number = 5;
        }

        [TearDown]
        public void Clean()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }

        [SetUp]
        public void SetUp()
        {
            _provider = new ProjectServiceDbContext(_dbContextOptions);
            _repository = new TaskRepository(_provider);

            _provider.Tasks.AddRange(_dbTasks);
            _provider.Save();
        }

        [Test]
        public void ShouldThrowExceptionWhenFilteIsNull()
        {
            FindTasksFilter filter = null;

            int skipCount = 0;
            int takeCount = _dbTasks.Count;

            Assert.Throws<ArgumentNullException>(() =>
            _repository.Find(filter, _projectIds, skipCount, takeCount, out int totalCount));
        }

        [Test]
        public void ShouldReturnFoundTasksSuccessfulWhenFindByAssign()
        {
            var filter = new FindTasksFilter();
            filter.Assign = _assign;

            int skipCount = 0;
            int takeCount = _dbTasks.Count;

            SerializerAssert.AreEqual(_dbTasks,
                _repository.Find(filter, _projectIds, skipCount, takeCount, out int totalCount).ToList());
            Assert.AreEqual(_dbTasks.Count, totalCount);
        }

        [Test]
        public void ShouldReturnFoundTasksSuccessfulWhenFindByNumber()
        {
            var filter = new FindTasksFilter();
            filter.Number = 2;

            int skipCount = 0;
            int takeCount = _dbTasks.Count;

            var expectedDbTaks = _dbTasks.Where(x => x.Number == filter.Number).ToList();

            SerializerAssert.AreEqual(expectedDbTaks,
                _repository.Find(filter, _projectIds, skipCount, takeCount, out int totalCount).ToList());
            Assert.AreEqual(_dbTasks.Count - 1, totalCount);
        }

        [Test]
        public void ShouldReturnFoundTasksSuccessfulWhenFindByProjectId()
        {
            var filter = new FindTasksFilter();
            filter.ProjectId = _projectId;

            int skipCount = 0;
            int takeCount = _dbTasks.Count;

            SerializerAssert.AreEqual(_dbTasks,
                _repository.Find(filter, _projectIds, skipCount, takeCount, out int totalCount).ToList());
            Assert.AreEqual(_dbTasks.Count, totalCount);
        }
    }
}
