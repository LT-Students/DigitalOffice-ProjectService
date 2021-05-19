using System;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class TaskResponseMapper : ITaskResponseMapper
    {
        public TaskResponse Map(DbTask dbTask)
        {
            if (dbTask == null)
            {
                throw new ArgumentNullException(nameof(dbTask));
            }

            return new TaskResponse()
            {
                Id = dbTask.Id,
                Type = dbTask.Type,
                Author = dbTask.Author,
                Status = dbTask.Status,
                ParentTask = dbTask.ParentTask,
                Project = dbTask.Project,
                Priority = dbTask.Priority,
                AssignedUser = dbTask.AssignedUser,
                Name = dbTask.Name,
                Description = dbTask.Description,
                Number = dbTask.Number,
                PlannedMinutes = dbTask.PlannedMinutes,
                CreatedAt = dbTask.CreatedAt,
                Subtasks = dbTask.Subtasks
            };
        }
    }
}