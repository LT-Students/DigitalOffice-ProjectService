using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Moq;
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
        private Mock<IHttpContextAccessor> _accessorMock;

        private Guid _creatorId = Guid.NewGuid();
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
                Guid projectId = Guid.NewGuid();
                Guid typeId = Guid.NewGuid();
                Guid statusId = Guid.NewGuid();
                Guid priorityId = Guid.NewGuid();

                _dbTasks.Add(
                    new DbTask
                    {
                        Id = Guid.NewGuid(),
                        Name = "Create Smth",
                        Description = "Create smth in somewhere",
                        PlannedMinutes = 30,
                        AssignedTo = _assign,
                        CreatedBy = Guid.NewGuid(),
                        ProjectId = projectId,
                        CreatedAtUtc = DateTime.UtcNow,
                        ParentId = Guid.NewGuid(),
                        Number = 2,
                        PriorityId = priorityId,
                        StatusId = statusId,
                        TypeId = typeId,
                        Project = new DbProject
                        {
                            Id = projectId
                        },
                        Status = new DbTaskProperty
                        {
                            Id = statusId,
                            Name = "In progress"
                        },
                        Priority = new DbTaskProperty
                        {
                            Id = priorityId,
                            Name = "Normal"
                        },
                        Type = new DbTaskProperty
                        {
                            Id = typeId,
                            Name = "Feature"
                        }
                    });
            }

            _dbTasks[3].Number = 5;
            _dbTasks[1].ProjectId = _projectId;
            _dbTasks[1].Project.Id = _projectId;
        }

        [SetUp]
        public void SetUp()
        {
            _accessorMock = new();
            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _creatorId);

            _accessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(_items);

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
            _repository = new TaskRepository(_provider, _accessorMock.Object);

            _provider.Tasks.AddRange(_dbTasks);
            _provider.Save();
        }

        /*[Test]
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
            filter.AssignedTo = _assign;

            int skipCount = 0;
            int takeCount = _dbTasks.Count;

            List<Guid> _expectedDbTaskIds = _dbTasks.Select(x => x.Id).ToList();
            List<Guid> _dbTaskIds = _repository.Find(filter, _projectIds, skipCount, takeCount, out int totalCount).Select(x => x.Id).ToList();

            SerializerAssert.AreEqual(_expectedDbTaskIds, _dbTaskIds);
            Assert.AreEqual(_dbTasks.Count, totalCount);
        }

        [Test]
        public void ShouldReturnFoundTasksSuccessfulWhenFindByNumber()
        {
            var filter = new FindTasksFilter();
            filter.Number = 2;

            int skipCount = 0;
            int takeCount = _dbTasks.Count;

            List<Guid> _expectedDbTaskIds = _dbTasks.Where(x => x.Number == filter.Number).Select(x => x.Id).ToList();

            List<Guid> _dbTaskIds = _repository.Find(filter, _projectIds, skipCount, takeCount, out int totalCount).Select(x => x.Id).ToList();

            SerializerAssert.AreEqual(_expectedDbTaskIds, _dbTaskIds);
            Assert.AreEqual(_dbTasks.Count - 1, totalCount);
        }

        [Test]
        public void ShouldReturnFoundTasksSuccessfulWhenFindByProjectId()
        {
            var filter = new FindTasksFilter();
            filter.ProjectId = _projectId;

            int skipCount = 0;
            int takeCount = _dbTasks.Count;
            List<Guid> _expectedDbTaskIds = _dbTasks.Where(x => x.ProjectId == _projectId).Select(x => x.Id).ToList();
            List<Guid> _dbTaskIds = _repository.Find(filter, _projectIds, skipCount, takeCount, out int totalCount).Select(x => x.Id).ToList();

            SerializerAssert.AreEqual(_expectedDbTaskIds, _dbTaskIds);
            Assert.AreEqual(_expectedDbTaskIds.Count, totalCount);
        }

        [Test]
        public void ShouldEditTask()
        {
            _provider.Tasks.Add(_dbTask);
            _provider.Save();
            Assert.IsTrue(_repository.Edit(_repository.Get(_taskId, false), _patchDbTask));
            SerializerAssert.AreEqual(_result, _provider.Tasks.FirstOrDefault(x => x.Id == _taskId));
        }*/

        [Test]
        public void ShouldCreateNewTask()
        {
            SerializerAssert.AreEqual(_dbTask.Id, _repository.Create(_dbTask));
            Assert.NotNull(_provider.Tasks.Find(_dbTask.Id));
        }

        /*[Test]
        public void ExceptionWhenThereIsNotTask()
        {
            _taskId = Guid.NewGuid();

            Assert.Throws<NotFoundException>(() => _repository.Get(_taskId, false));

            Assert.Throws<NotFoundException>(() => _repository.Edit(_repository.Get(_taskId, false), _patchDbTask));
        }*/

        [Test]
        public void ShouldGetTask()
        {
            _provider.Tasks.Add(_dbTask);
            _provider.Save();
            Assert.AreEqual(_dbTask, _repository.Get(_taskId, false));
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
