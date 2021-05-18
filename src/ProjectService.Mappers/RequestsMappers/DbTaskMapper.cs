using LT.DigitalOffice.ProjectService.Mappers.Helpers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class DbTaskMapper : IDbTaskMapper
    {
        public DbTask Map(CreateTaskRequest taskRequest)
        {
            if (taskRequest == null)
            {
                throw new ArgumentNullException(nameof(taskRequest));
            }

            Guid taskId = Guid.NewGuid();

            return new DbTask
            {
                Id = taskId,
                Name = taskRequest.Name,
                Description = taskRequest.Description,
                PlannedMinutes = taskRequest.PlannedMinutes,
                AssignedTo = taskRequest.AssignedTo,
                AuthorId = taskRequest.AuthorId,
                ProjectId = taskRequest.ProjectId,
                CreatedAt = DateTime.UtcNow,
                ParentId = taskRequest.ParentId,
                Number = TaskNumberHelper.GetProjectTaskNumber(taskRequest.ProjectId),
                Priority = new DbTaskProperty
                {
                    Id = taskRequest.PriorityId
                },
                Status = new DbTaskProperty
                {
                    Id = taskRequest.StatusId
                },
                Type = new DbTaskProperty
                {
                    Id = taskRequest.TypeId
                }
            };
        }
    }
}
