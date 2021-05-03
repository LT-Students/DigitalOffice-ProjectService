using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
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
                    ProjectName = "DO"
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
            IGetUserDataResponse userData= null;

            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, userData, userData));
        }

        [Test]
        public void ShouldReturnDbTaskSuccessful()
        {
            var assignedUserData = new Mock<IGetUserDataResponse>();
            var authorData = new Mock<IGetUserDataResponse>();

            assignedUserData.Setup(x => x.FirstName).Returns("Ivan");
            assignedUserData.Setup(x => x.LastName).Returns("Ivanov");

            authorData.Setup(x => x.FirstName).Returns("Semen");
            authorData.Setup(x => x.LastName).Returns("Semenov");

            SerializerAssert.AreEqual(_taskInfo, _mapper.Map(_dbTask, assignedUserData.Object, authorData.Object));
        }
    }
}
