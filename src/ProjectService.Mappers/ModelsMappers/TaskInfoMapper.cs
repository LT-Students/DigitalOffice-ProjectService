using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class TaskInfoMapper : ITaskInfoMapper
    {
        public TaskInfo Map(DbTask dbTask)
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
                TypeId = dbTask.TypeId,
                StatusId = dbTask.StatusId,
                Deadline = dbTask.Deadline,
                ParentId = dbTask.ParentId,
                AuthorId = dbTask.AuthorId,
                ProjectId = dbTask.ProjectId,
                CreatedAt = dbTask.CreatedAt,
                AssignedTo = dbTask.AssignedTo,
                PriorityId = dbTask.PriorityId,
                Description = dbTask.Description,
                PlannedMinutes = dbTask.PlannedMinutes,
            };
        }
    }
}
