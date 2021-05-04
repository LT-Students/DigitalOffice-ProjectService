using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    internal class CreateTaskRepositoryTests
    {
        private IDataProvider _provider;
        private ITaskRepository _taskRepository;

        private DbTask newTask;
        private readonly Guid _Id = Guid.NewGuid();
        private readonly Guid _parentTaskId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            var dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase("CreateTaskTest")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptionsProjectService);

            _taskRepository = new TaskRepository(_provider);

            newTask = new DbTask
            {
                Id = _Id,
                Name = "DigitalOffice",
                ProjectId = Guid.NewGuid(),
                AssignedTo = Guid.NewGuid(),
                Description = "Add smth model to Db",
                TypeId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                PlannedMinutes = 30,
                ParentId = _parentTaskId,
                AuthorId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Number = 3
            };
        }

        [Test]
        public void ShouldAddNewTaskToDbAndCheckPerentIdExistanse()
        {
            SerializerAssert.AreEqual(newTask.Id, _taskRepository.CreateTask(newTask));
            Assert.NotNull(_provider.Tasks.Find(newTask.Id));
            Assert.IsTrue(_taskRepository.IsExist(newTask.ParentId.Value));
        }

        [TearDown]
        public void CleanMemoryDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }
    }
}

