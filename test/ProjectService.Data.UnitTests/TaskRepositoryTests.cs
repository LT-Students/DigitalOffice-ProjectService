using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
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
        
        private Guid _taskId = Guid.NewGuid();
        private string _name = "NewName";
        private string _description = "New Description";
        private Guid _assignedTo = Guid.NewGuid();
        private int _plannedMinutes = 60;
        private Guid _priorityId = Guid.NewGuid();
        private Guid _statusId = Guid.NewGuid();
        private Guid _typeId = Guid.NewGuid();
        private void CreateMemoryDb()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptions);
            
            _provider.Tasks.Add(new DbTask()
            {
                Id = _taskId,
                Name = "Name",
                Description = "Description",
                AssignedTo = Guid.NewGuid(),
                PlannedMinutes = 60,
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            });
            
            _provider.Save();
            
            _repository = new TaskRepository(_provider);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
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

        [TearDown]
        public void CleanDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }

        [Test]
        public void ShouldEditTask()
        {
            Assert.IsTrue(_repository.Edit(_taskId, _patchDbTask));
            SerializerAssert.AreEqual(_result, _provider.Tasks.FirstOrDefault(x => x.Id == _taskId));
        }
    }
}