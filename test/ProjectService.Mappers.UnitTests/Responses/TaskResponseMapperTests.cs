using System;
using System.Collections.Generic;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Responses
{
    public class TaskResponseMapperTests
    {
        private DbTask _dbTask;
        private TaskResponse _taskResponse;
        private IProjectInfoMapper _projectInfoMapper = new ProjectInfoMapper();
        private ITaskInfoMapper _taskInfoMapper = new TaskInfoMapper();
        private ITaskPropertyInfoMapper _taskPropertyInfoMapper = new TaskPropertyInfoMapper();
        private IProjectUserInfoMapper _projectUserInfoMapper = new ProjectUserInfoMapper();

        private ITaskResponseMapper _mapper;

        private readonly Guid _taskId = Guid.NewGuid();
        private readonly string _name = "Name";
        private readonly string _description = "Description";
        private readonly int _number = 1;
        private readonly int _plannedMinutes = 60;
        private readonly DateTime _createdAt = DateTime.UtcNow;
        private Guid _userId = Guid.NewGuid();
        private string _firstName = "Name";
        private string _middleName = "Middle Name";
        private string _lastName = "Last Name";
        private bool _isActive = true;

        private UserData _authorUserData;
        private UserData _parentUserData;
        private UserData _parentAssignedUserData;
        private UserData _parentAuthorAssignedUserData;
        private readonly string _departmentName = "Department name";
        private UserData _assignedUserData;

        private readonly ICollection<TaskInfo> _subtasksInfo = new List<TaskInfo>()
        {
            new(){Id = Guid.NewGuid()},
            new(){Id = Guid.NewGuid()},
            new(){Id = Guid.NewGuid()},
            new(){Id = Guid.NewGuid()},
        };

        private readonly ProjectInfo _projectInfo = new();
        private ProjectUserInfo _authorProjectUserInfo;
        private ProjectUserInfo _assignedProjectUserInfo;
        private readonly TaskPropertyInfo _statusPropertyInfo = new();
        private readonly TaskPropertyInfo _priorityPropertyInfo = new();
        private readonly TaskPropertyInfo _typePropertyInfo = new();
        private TaskInfo _parentTaskInfo;
        private readonly List<TaskInfo> _subtasksInfos = new();

        private readonly DbProject _project = new();
        private DbProjectUser _author;
        private DbProjectUser _assignedUser;
        private readonly DbTaskProperty _status = new();
        private readonly DbTaskProperty _priority = new();
        private readonly DbTaskProperty _type = new();
        private DbTask _parentTask;

        private readonly List<DbTask> _subtasks = new();

        [SetUp]
        public void SetUp()
        {
            _parentTask = new()
            {
                AuthorId = _userId
            };
            
            _parentTaskInfo = new TaskInfo()
            {
                Author = new UserTaskInfo()
                {
                    FirstName = _firstName,
                    Id = _userId,
                    LastName = _lastName
                } 
            };
            
            _authorProjectUserInfo = new ProjectUserInfo()
            {
                Id = _userId,
                FirstName = _firstName,
                MiddleName = _middleName,
                LastName = _lastName,
                IsActive = _isActive
            };

            _assignedProjectUserInfo = new ProjectUserInfo()
            {
                Id = _userId,
                FirstName = _firstName,
                MiddleName = _middleName,
                LastName = _lastName,
                IsActive = _isActive
            };
            
            _author = new DbProjectUser()
            {
                UserId = _userId,
                IsActive = _isActive
            };

            _assignedUser = new DbProjectUser()
            {
                UserId = _userId,
                IsActive = _isActive
            };

            _authorUserData = new UserData(_userId, _firstName, _middleName, _lastName, _isActive);
            _parentUserData = new UserData(_userId, _firstName, _middleName, _lastName, _isActive);
            _parentAssignedUserData = new UserData(_userId, _firstName, _middleName, _lastName, _isActive);
            _parentAuthorAssignedUserData = new UserData(_userId, _firstName, _middleName, _lastName, _isActive);
            _assignedUserData = new UserData(_userId, _firstName, _middleName, _lastName, _isActive);

            _mapper = new TaskResponseMapper(
                _projectInfoMapper,
                _taskInfoMapper,
                _taskPropertyInfoMapper,
                _projectUserInfoMapper);

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
                Project = _projectInfo,
                Author = _authorProjectUserInfo,
                AssignedUser = _assignedProjectUserInfo,
                Status = _statusPropertyInfo,
                Priority = _priorityPropertyInfo,
                Type = _typePropertyInfo,
                ParentTask = _parentTaskInfo,
                Subtasks = _subtasksInfos
            };
        }

        [Test]
        public void ShouldReturnTaskResponse()
        {
            TaskResponse result = _mapper.Map(
                _dbTask,
                _authorUserData,
                _parentAssignedUserData,
                _parentAuthorAssignedUserData,
                _departmentName,
                _assignedUserData,
                _subtasksInfos
            );
            
            SerializerAssert.AreEqual(
                _taskResponse,
                result
            );
        }

        [Test]
        public void ExceptionWhenArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(
                null,
                null,
                null,
                null,
                null,
                null,
                null));
        }
    }
}