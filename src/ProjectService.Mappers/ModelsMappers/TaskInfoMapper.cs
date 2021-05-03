using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class TaskInfoMapper : ITaskInfoMapper
    {
        public TaskInfo Map(DbTask dbTask, IGetUserDataResponse assignedUser, IGetUserDataResponse author)
        {
            if (dbTask == null)
            {
                throw new ArgumentNullException(nameof(dbTask));
            }

            var db = new TaskInfo
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
                    ProjectName = dbTask.Project.ShortName
                },
                AssignedTo = new UserTaskInfo
                {
                    Id = dbTask.AssignedTo.HasValue ? dbTask.AssignedTo.Value : null,
                    FirstName = assignedUser?.FirstName,
                    LastName = assignedUser?.LastName
                }
            };

            return db;
        }
    }
}
