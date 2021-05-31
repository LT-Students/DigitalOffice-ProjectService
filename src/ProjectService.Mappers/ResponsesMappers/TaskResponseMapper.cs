using System;
using System.Collections.Generic;
using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
 
namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class TaskResponseMapper : ITaskResponseMapper
    {
        private readonly IProjectInfoMapper _projectInfoMapper;
        private readonly ITaskInfoMapper _taskInfoMapper;
        private readonly ITaskPropertyInfoMapper _taskPropertyInfoMapper;
        private readonly IProjectUserInfoMapper _projectUserInfoMapper;

        public TaskResponseMapper(
            IProjectInfoMapper projectInfoMapper,
            ITaskInfoMapper taskInfoMapper,
            ITaskPropertyInfoMapper taskPropertyInfoMapper,
            IProjectUserInfoMapper projectUserInfoMapper)
        {
            _projectInfoMapper = projectInfoMapper;
            _taskInfoMapper = taskInfoMapper;
            _taskPropertyInfoMapper = taskPropertyInfoMapper;
            _projectUserInfoMapper = projectUserInfoMapper;
        }

        public TaskResponse Map(
            DbTask dbTask,
            UserData authorUserData, 
            UserData parentAssignedUserData,
            UserData parentAuthorAssignedUserData, 
            string departmentName,
            UserData assignedUserData,
            ICollection<TaskInfo> subtasksInfo)
        {
            if (dbTask == null)
            {
                throw new ArgumentNullException(nameof(dbTask));
            }
 
            return new TaskResponse()
            {
                Id = dbTask.Id,
                Type = _taskPropertyInfoMapper.Map(dbTask.Type),
                Author = _projectUserInfoMapper.Map(authorUserData, dbTask.Author),
                Status = _taskPropertyInfoMapper.Map(dbTask.Status),
                ParentTask = _taskInfoMapper.Map(
                    dbTask.ParentTask,
                    parentAssignedUserData,
                    parentAuthorAssignedUserData),
                Project = _projectInfoMapper.Map(dbTask.Project, departmentName),
                Priority = _taskPropertyInfoMapper.Map(dbTask.Priority),
                AssignedUser = _projectUserInfoMapper.Map(assignedUserData, dbTask.AssignedUser),
                Name = dbTask.Name,
                Description = dbTask.Description,
                Number = dbTask.Number,
                PlannedMinutes = dbTask.PlannedMinutes,
                CreatedAt = dbTask.CreatedAt,
                Subtasks = subtasksInfo
            };
        }
    }
}
