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

        private readonly Guid authorId = Guid.NewGuid();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbTaskMapper = new DbTaskMapper();

            _createTaskRequest = new CreateTaskRequest
            {
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = Guid.NewGuid(),
                AuthorId = authorId,
                Deadline = DateTime.UtcNow,
                ProjectId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ParentTaskId = Guid.NewGuid(),
                Number = 2,
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenCreateTaskRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _dbTaskMapper.Map(null, authorId));
        }

        [Test]
        public void ShouldReturnDbTaskWhenCreateTaskRequestIsMapped()
        {
            var authorId = Guid.NewGuid();

            var dbTask = _dbTaskMapper.Map(_createTaskRequest, authorId);

            var expectedDbTask = new DbTask
            {
                Id = dbTask.Id,
                Name = _createTaskRequest.Name,
                Description = _createTaskRequest.Description,
                PlannedMinutes = _createTaskRequest.PlannedMinutes,
                AssignedTo = _createTaskRequest.AssignedTo,
                AuthorId =authorId,
                Deadline = _createTaskRequest.Deadline,
                ProjectId = _createTaskRequest.ProjectId,
                CreatedAt = dbTask.CreatedAt,
                ParentTaskId = _createTaskRequest.ParentTaskId,
                Number = _createTaskRequest.Number,
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

            SerializerAssert.AreEqual(expectedDbTask, dbTask);
        }
    }
}
