using LT.DigitalOffice.Models.Broker.Models;
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
                TypeName = dbTask.Type?.Name,
                CreatedAt = dbTask.CreatedAt,
                StatusName = dbTask.Status?.Name,
                Description = dbTask.Description,
                PriorityName = dbTask.Priority?.Name,
                PlannedMinutes = dbTask.PlannedMinutes,
                Author = author != null
                    ? new UserTaskInfo
                    {
                        Id = dbTask.AuthorId,
                        FirstName = author.FirstName,
                        LastName = author.LastName
                    }
                    : null,
                Project = dbTask.Project != null
                    ? new ProjectTaskInfo
                    {
                        Id = dbTask.ProjectId,
                        ShortName = dbTask.Project.ShortName
                    }
                    : null,
                AssignedTo = assignedUser != null && dbTask.AssignedTo.HasValue
                    ? new UserTaskInfo
                    {
                        Id = dbTask.AssignedTo.HasValue ? dbTask.AssignedTo.Value : null,
                        FirstName = assignedUser?.FirstName,
                        LastName = assignedUser?.LastName
                    }
                    : null
            };
        }
    }
}
