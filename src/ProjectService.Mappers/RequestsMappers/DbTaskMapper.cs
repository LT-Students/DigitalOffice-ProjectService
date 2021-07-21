using LT.DigitalOffice.ProjectService.Mappers.Helpers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class DbTaskMapper : IDbTaskMapper
    {
        public DbTask Map(CreateTaskRequest taskRequest, Guid authorId)
        {
            if (taskRequest == null)
            {
                throw new ArgumentNullException(nameof(taskRequest));
            }

            return new DbTask
            {
                Id = Guid.NewGuid(),
                Name = taskRequest.Name,
                Description = string.IsNullOrEmpty(taskRequest.Description?.Trim()) ? null : taskRequest.Description.Trim(),
                PlannedMinutes = taskRequest.PlannedMinutes,
                AssignedTo = taskRequest.AssignedTo,
                AuthorId = authorId,
                ProjectId = taskRequest.ProjectId,
                CreatedAt = DateTime.UtcNow,
                ParentId = taskRequest.ParentId,
                PriorityId = taskRequest.PriorityId,
                StatusId = taskRequest.StatusId,
                TypeId = taskRequest.TypeId,
                Number = TaskNumberHelper.GetProjectTaskNumber(taskRequest.ProjectId)
            };
        }
    }
}
