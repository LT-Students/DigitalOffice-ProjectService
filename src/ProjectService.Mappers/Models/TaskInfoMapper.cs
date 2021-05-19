﻿using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class TaskInfoMapper : ITaskInfoMapper
    {
        public TaskInfo Map(DbTask dbTask, UserData assignedUser, UserData author)
        {
            if (dbTask == null)
            {
                throw new ArgumentNullException(nameof(dbTask));
            }

            return new TaskInfo
            {
                Id = dbTask.Id,
                Name = dbTask.Name,
                Number = dbTask.Number,
                TypeName = dbTask.Type.Name,
                CreatedAt = dbTask.CreatedAt,
                StatusName = dbTask.Status.Name,
                Description = dbTask.Description,
                PriorityName = dbTask.Priority.Name,
                PlannedMinutes = dbTask.PlannedMinutes,
                Author = new UserTaskInfo
                {
                    Id = dbTask.AuthorId,
                    FirstName = author?.FirstName,
                    LastName = author?.LastName
                },
                Project = new ProjectTaskInfo
                {
                    Id = dbTask.ProjectId,
                    ShortName = dbTask.Project.ShortName
                },
                AssignedTo = new UserTaskInfo
                {
                    Id = dbTask.AssignedTo.HasValue ? dbTask.AssignedTo.Value : null,
                    FirstName = assignedUser?.FirstName,
                    LastName = assignedUser?.LastName
                }
            };
        }
    }
}