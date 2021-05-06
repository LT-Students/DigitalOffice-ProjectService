using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    internal class TaskRepositoryTests
    {
        private IDataProvider _provider;
        private ITaskRepository _repository;

        private JsonPatchDocument<DbTask> _patchDbTask;
        private DbTask _result;

        private Guid _taskId;
        private readonly string _name = "NewName";
        private readonly string _description = "New Description";
        private readonly Guid _assignedTo = Guid.NewGuid();
        private readonly int _plannedMinutes = 60;
        private readonly Guid _priorityId = Guid.NewGuid();
        private readonly Guid _statusId = Guid.NewGuid();
        private readonly Guid _typeId = Guid.NewGuid();

        private DbTask _dbTask;
        
        private void CreateMemoryDb()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptions);

            _provider.Tasks.Add(_dbTask);

            _provider.Save();

            _repository = new TaskRepository(_provider);
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
                
            CreateMemoryDb();
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
        }

        [Test]
        public void ShouldEditTask()
        {
            Assert.IsTrue(_repository.Edit(_repository.Get(_taskId), _patchDbTask));
            SerializerAssert.AreEqual(_result, _provider.Tasks.FirstOrDefault(x => x.Id == _taskId));
        }

        [Test]
        public void ExceptionWhenThereIsNotTask()
        {
            _taskId = Guid.NewGuid();

            Assert.Throws<NotFoundException>(() => _repository.Get(_taskId));

            Assert.Throws<NotFoundException>(() => _repository.Edit(_repository.Get(_taskId), _patchDbTask));
        }
        
        [Test]
        public void ShouldGetTask()
        {
            Assert.AreEqual(_dbTask, _repository.Get(_taskId));
        }

        [TearDown]
        public void CleanDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }
    }
}
