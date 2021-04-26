using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.RequestsMappers
{
    internal class DbTaskMapperTests
    {
        private IDbTaskMapper _dbTaskMapper;
        private CreateTaskRequest _createTaskRequest;
        public DbTask _dbTask;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbTaskMapper = new DbTaskMapper();

            _createTaskRequest = new CreateTaskRequest
            {
                Id = Guid.NewGuid(),
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Deadline = DateTime.UtcNow,
                ProjectId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ParentTaskId = Guid.NewGuid(),
                Number = 2,
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            };

            _dbTask = new DbTask
            {
                Id = _createTaskRequest.Id,
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = _createTaskRequest.AssignedTo,
                AuthorId = _createTaskRequest.AuthorId,
                Deadline = _createTaskRequest.Deadline,
                ProjectId = _createTaskRequest.ProjectId,
                CreatedAt = _createTaskRequest.CreatedAt,
                ParentTaskId =_createTaskRequest.ParentTaskId,
                Number = 2,
                Priority = new DbTaskProperty()
                {
                    Id = _createTaskRequest.PriorityId
                },
                Status = new DbTaskProperty()
                {
                    Id = _createTaskRequest.StatusId
                },
                Type = new DbTaskProperty()
                {
                    Id = _createTaskRequest.TypeId
                }
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenCreateTaskRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _dbTaskMapper.Map(null));
        }

        [Test]
        public void ShouldReturnDbTaskWhenCreateTaskRequestIsMapped()
        {
            SerializerAssert.AreEqual(_dbTask, _dbTaskMapper.Map(_createTaskRequest));
        }
    }
}
