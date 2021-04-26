using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    internal class CreateTaskRepositoryTests
    {
        private IDataProvider _provider;
        private ITaskRepository _taskRepository;

        private DbTask newTask;

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
                Id = Guid.NewGuid(),
                Name = "DigitalOffice",
                ProjectId = Guid.NewGuid(),
                AssignedTo = Guid.NewGuid(),
                Description = "Add smth model to Db",
                TypeId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                Deadline = DateTime.UtcNow,
                PlannedMinutes =30,
                ParentTaskId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Number = 3
            };

            _taskRepository.CreateNewTask(newTask);
        }

        [Test]
        public void ShouldAddNewTaskWhenTheNameWasRepeated()
        {
            var newTaskWithRepeatedName = newTask;
            newTaskWithRepeatedName.Id = Guid.NewGuid();

            Assert.That(_taskRepository.CreateNewTask(newTask), Is.EqualTo(newTaskWithRepeatedName.Id));
            SerializerAssert.AreEqual(newTaskWithRepeatedName, _provider.Task.FirstOrDefault(task => task.Id == newTaskWithRepeatedName.Id));
        }

     /*   [Test]
        public void ShouldAddNewTaskToDb()
        {
            newTask.Name = "Any name";
            newTask.Id = Guid.NewGuid();

            Assert.AreEqual(newTask.Id, _taskRepository.CreateNewTask(newTask));
            Assert.That(_provider.Projects.Find(newTask.Id), Is.EqualTo(newTask));
        }*/

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

