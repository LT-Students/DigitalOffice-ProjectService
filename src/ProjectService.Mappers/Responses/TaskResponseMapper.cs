using System;
using System.Collections.Generic;
using LT.DigitalOffice.Models.Broker.Models;
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
                Type = dbTask.Type != null 
                    ? _taskPropertyInfoMapper.Map(dbTask.Type)
                    : null,
                Author = dbTask.Author != null 
                    ? _projectUserInfoMapper.Map(authorUserData, dbTask.Author)
                    : null,
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
                    ? _projectInfoMapper.Map(dbTask.Project, departmentName) 
                    : null,
                Priority = dbTask.Priority != null 
                    ? _taskPropertyInfoMapper.Map(dbTask.Priority) 
                    : null,
                AssignedUser = assignedUserData != null
                    ? _projectUserInfoMapper.Map(assignedUserData, dbTask.AssignedUser)
                    : null,
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