using System;
using System.Collections.Generic;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.Responses
{
    public class TaskResponseMapper : ITaskResponseMapper
    {
        private readonly IProjectInfoMapper _projectInfoMapper;
        private readonly ITaskInfoMapper _taskInfoMapper;
        private readonly IDepartmentInfoMapper _departmentInfoMapper;
        private readonly ITaskPropertyInfoMapper _taskPropertyInfoMapper;
        private readonly IUserTaskInfoMapper _userTaskInfoMapper;

        public TaskResponseMapper(
            IProjectInfoMapper projectInfoMapper,
            ITaskInfoMapper taskInfoMapper,
            IDepartmentInfoMapper departmentInfoMapper,
            ITaskPropertyInfoMapper taskPropertyInfoMapper,
            IUserTaskInfoMapper userTaskInfoMapper)
        {
            _projectInfoMapper = projectInfoMapper;
            _taskInfoMapper = taskInfoMapper;
            _departmentInfoMapper = departmentInfoMapper;
            _taskPropertyInfoMapper = taskPropertyInfoMapper;
            _userTaskInfoMapper = userTaskInfoMapper;
        }

        public TaskResponse Map(
            DbTask dbTask,
            UserData authorUserData,
            UserData parentAssignedUserData,
            UserData parentAuthorAssignedUserData,
            DepartmentData department,
            UserData assignedUserData,
            ICollection<TaskInfo> subtasksInfo,
            List<ImageInfo> imagesInfo)
        {
            if (dbTask == null)
            {
                throw new ArgumentNullException(nameof(dbTask));
            }

            return new TaskResponse()
            {
                Id = dbTask.Id,
                Type = dbTask.Type != null
                    ? _taskPropertyInfoMapper.Map(dbTask.Type)
                    : null,
                CreatedBy = _userTaskInfoMapper.Map(authorUserData),
                Status = dbTask.Status != null
                    ? _taskPropertyInfoMapper.Map(dbTask.Status)
                    : null,
                ParentTask = dbTask.ParentTask != null
                    ? _taskInfoMapper.Map(
                        dbTask.ParentTask,
                        parentAssignedUserData,
                        parentAuthorAssignedUserData)
                    : null,
                Project = dbTask.Project != null
                    ? _projectInfoMapper.Map(dbTask.Project, _departmentInfoMapper.Map(department))
                    : null,
                Priority = dbTask.Priority != null
                    ? _taskPropertyInfoMapper.Map(dbTask.Priority)
                    : null,
                AssignedUser = _userTaskInfoMapper.Map(assignedUserData),
                Name = dbTask.Name,
                Description = dbTask.Description,
                Number = dbTask.Number,
                PlannedMinutes = dbTask.PlannedMinutes,
                CreatedAtUtc = dbTask.CreatedAtUtc,
                Subtasks = subtasksInfo,
                TasksImages = imagesInfo
            };
        }
    }
}
