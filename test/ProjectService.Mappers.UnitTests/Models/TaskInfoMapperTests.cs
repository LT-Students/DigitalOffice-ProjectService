using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
{
    class TaskInfoMapperTests
    {
        private ITaskInfoMapper _mapper;
        private TaskInfo _taskInfo;
        public DbTask _dbTask;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new TaskInfoMapper();

            var projectId = Guid.NewGuid();

            _dbTask = new DbTask
            {
                Id = Guid.NewGuid(),
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                ParentId = Guid.NewGuid(),
                Number = 2,
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                Type = new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = "Feature"
                },
                Status = new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = "New"
                },
                Priority = new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = "First"
                },
                Project = new DbProject
                {
                    Id = projectId,
                    ShortName = "DO"
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            _taskInfo = new TaskInfo
            {
                Id = _dbTask.Id,
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = new UserTaskInfo
                {
                    Id = _dbTask.AssignedTo,
                    FirstName = "Ivan",
                    LastName = "Ivanov"
                },
                Author = new UserTaskInfo
                {
                    Id = _dbTask.AuthorId,
                    FirstName = "Semen",
                    LastName = "Semenov"
                },
                Project = new ProjectTaskInfo
                {
                    Id = _dbTask.ProjectId,
                    ShortName = "DO"
                },
                CreatedAt = _dbTask.CreatedAt,
                Number = 2,
                PriorityName = "First",
                StatusName = "New",
                TypeName = "Feature"
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbTaskIsNull()
        {
            UserData userData = null;

            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, userData, userData));
        }

        [Test]
        public void ShouldReturnDbTaskSuccessfulWhenUserDataIsNull()
        {
            UserData assignedUserData = null;
            UserData authorData = null;

            _taskInfo.AssignedTo.FirstName = null;
            _taskInfo.AssignedTo.LastName = null;

            _taskInfo.Author.FirstName = null;
            _taskInfo.Author.LastName = null;

            SerializerAssert.AreEqual(_taskInfo, _mapper.Map(_dbTask, assignedUserData, authorData));
        }

        [Test]
        public void ShouldReturnDbTaskSuccessful()
        {
            UserData assignedUserData = new UserData
            {
                Id = Guid.NewGuid(),
                FirstName = "Ivan",
                LastName = "Ivanov"
            };

            UserData authorData = new UserData
            {
                Id = Guid.NewGuid(),
                FirstName = "Semen",
                LastName = "Semenov"
            };

            SerializerAssert.AreEqual(_taskInfo, _mapper.Map(_dbTask, assignedUserData, authorData));
        }
    }
}
