using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
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

        private DbTask _result;
        private DbTask _dbTask;
        private JsonPatchDocument<DbTask> _patchDbTask;

        private Guid _taskId;
        private string _name = "NewName";
        private string _description = "New Description";
        private Guid _assignedTo = Guid.NewGuid();
        private int _plannedMinutes = 60;
        private Guid _priorityId = Guid.NewGuid();
        private Guid _statusId = Guid.NewGuid();
        private Guid _typeId = Guid.NewGuid();

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

        [SetUp]
        public void SetUp()
        {
            _taskId = Guid.NewGuid();

            _dbTask = new DbTask()
            {
                Id = _taskId,
                Name = "Name",
                Description = "Description",
                AssignedTo = Guid.NewGuid(),
                PlannedMinutes = 60,
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            };

            _patchDbTask = new JsonPatchDocument<DbTask>(new List<Operation<DbTask>>()
            {
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.Name)}",
                    "",
                    _name),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.Description)}",
                    "",
                    _description),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.AssignedTo)}",
                    "",
                    _assignedTo),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.PlannedMinutes)}",
                    "",
                    _plannedMinutes),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.PriorityId)}",
                    "",
                    _priorityId),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.StatusId)}",
                    "",
                    _statusId),
                new Operation<DbTask>(
                    "replace",
                    $"/{nameof(DbTask.TypeId)}",
                    "",
                    _typeId),
            }, new CamelCasePropertyNamesContractResolver());

            _result = new DbTask()
            {
                Id = _taskId,
                Name = _name,
                Description = _description,
                AssignedTo = _assignedTo,
                PlannedMinutes = _plannedMinutes,
                PriorityId = _priorityId,
                StatusId = _statusId,
                TypeId = _typeId
            };

            _provider = new ProjectServiceDbContext(_dbContextOptions);
            _repository = new TaskRepository(_provider);

            _provider.Tasks.Add(_dbTask);
            _provider.Tasks.AddRange(_dbTasks);
            _provider.Save();
        }

        [Test]
        public void ShouldThrowExceptionWhenFilterIsNull()
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
            filter.AssignedTo = _assign;

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
        
        [Test]
        public void ExceptionWhenThereIsNotTask()
        {
            _taskId = Guid.NewGuid();

            Assert.Throws<NotFoundException>(() => _repository.Get(_taskId));

            Assert.Throws<NotFoundException>(() => _repository.Edit(_repository.Get(_taskId), _patchDbTask));
        }

        [Test]
        public void ShouldEditTask()
        {
            Assert.IsTrue(_repository.Edit(_repository.Get(_taskId), _patchDbTask));
            SerializerAssert.AreEqual(_result, _provider.Tasks.FirstOrDefault(x => x.Id == _taskId));
        }

        [Test]
        public void ShouldGetTask()
        {
            SerializerAssert.AreEqual(_dbTask, _repository.Get(_taskId));
        }

        [Test]
        public void ExceptionWhenGetNonexistentTask()
        {
            Assert.Throws<NotFoundException>(() => _repository.Get(Guid.NewGuid()));
        }

        [TearDown]
        public void Clean()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }
    }
}