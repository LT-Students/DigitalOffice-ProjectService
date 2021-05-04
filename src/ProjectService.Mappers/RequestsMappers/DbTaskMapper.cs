using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class DbTaskMapper : IDbTaskMapper
    {
        public DbTask Map(CreateTaskRequest taskRequiest, Guid authorId)
        {
            if (taskRequiest == null)
            {
                throw new ArgumentNullException(nameof(taskRequiest));
            }

            Guid taskId = Guid.NewGuid();

            return new DbTask
            {
                Id = taskId,
                Name = taskRequiest.Name,
                Description = taskRequiest.Description,
                PlannedMinutes = taskRequiest.PlannedMinutes,
                AssignedTo = taskRequiest.AssignedTo,
                AuthorId = authorId,
                ProjectId = taskRequiest.ProjectId,
                CreatedAt = DateTime.UtcNow,
                ParentId = taskRequiest.ParentId,
                Number = 1,
                Priority = new DbTaskProperty
                {
                    Id = taskRequiest.PriorityId
                },
                Status = new DbTaskProperty
                {
                    Id = taskRequiest.StatusId
                },
                Type = new DbTaskProperty
                {
                    Id = taskRequiest.TypeId
                }
            };
        }
    }
}
