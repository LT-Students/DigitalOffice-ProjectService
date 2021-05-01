using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            _taskInfo = new TaskInfo
            {
                Id = Guid.NewGuid(),
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ParentId = Guid.NewGuid(),
                Number = 2,
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            };

            _dbTask = new DbTask
            {
                Id = _taskInfo.Id,
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = _taskInfo.AssignedTo,
                AuthorId = _taskInfo.AuthorId,
                ProjectId = _taskInfo.ProjectId,
                CreatedAt = _taskInfo.CreatedAt,
                ParentId = _taskInfo.ParentId,
                Number = 2,
                PriorityId = _taskInfo.PriorityId,
                StatusId = _taskInfo.StatusId,
                TypeId =  _taskInfo.TypeId
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbTaskIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
        }

        [Test]
        public void ShouldReturnDbTaskSuccessful()
        {
            SerializerAssert.AreEqual(_taskInfo, _mapper.Map(_dbTask));
        }
    }
}
