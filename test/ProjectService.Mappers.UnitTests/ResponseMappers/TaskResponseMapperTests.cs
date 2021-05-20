using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.ResponseMappers
{
    public class TaskResponseMapperTests
    {
        private DbTask _dbTask;
        private TaskResponse _taskResponse;
        private readonly ITaskResponseMapper _mapper = new TaskResponseMapper();
        
        private readonly Guid _taskId = Guid.NewGuid();
        private readonly string _name = "Name";
        private readonly string _description = "Description";
        private readonly int _number = 1;
        private readonly int _plannedMinutes = 60;
        private readonly DateTime _createdAt = DateTime.UtcNow;
        
        private readonly DbProject _project = new ();
        private readonly DbProjectUser _author = new ();
        private readonly DbProjectUser _assignedUser = new ();
        private readonly DbTaskProperty _status = new ();
        private readonly DbTaskProperty _priority = new ();
        private readonly DbTaskProperty _type = new ();
        private readonly DbTask _parentTask = new ();
        
        private readonly List<DbTask> _subtasks = new ();

        [SetUp]
        public void SetUp()
        {
            _dbTask = new()
            {
                Id = _taskId,
                Name = _name,
                Description = _description,
                Number = _number,
                PlannedMinutes = _plannedMinutes,
                CreatedAt = _createdAt,
                Project = _project,
                Author = _author,
                AssignedUser = _assignedUser,
                Status = _status,
                Priority = _priority,
                Type = _type,
                ParentTask = _parentTask,
                Subtasks = _subtasks
            };
            
            _taskResponse = new()
            {
                Id = _taskId,
                Name = _name,
                Description = _description,
                Number = _number,
                PlannedMinutes = _plannedMinutes,
                CreatedAt = _createdAt,
                Project = _project,
                Author = _author,
                AssignedUser = _assignedUser,
                Status = _status,
                Priority = _priority,
                Type = _type,
                ParentTask = _parentTask,
                Subtasks = _subtasks
            };
        }

        [Test]
        public void ShouldReturnTaskResponse()
        {
            SerializerAssert.AreEqual(_taskResponse, _mapper.Map(_dbTask));
        }

        [Test]
        public void ExceptionWhenArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
        }
    }
}
