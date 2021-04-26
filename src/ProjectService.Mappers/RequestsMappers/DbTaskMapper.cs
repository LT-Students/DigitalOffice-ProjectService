using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class DbTaskMapper : IDbTaskMapper
    {
        public DbTask Map(CreateTaskRequest taskRequiest)
        {
            if (taskRequiest == null)
            {
                throw new ArgumentNullException(nameof(taskRequiest));
            }

            return new DbTask
            {
                Id = taskRequiest.Id,
                Name = taskRequiest.Name,
                Description = taskRequiest.Description,
                PlannedMinutes = taskRequiest.PlannedMinutes,
                AssignedTo = taskRequiest.AssignedTo,
                AuthorId = taskRequiest.AuthorId,
                Deadline = taskRequiest.Deadline,
                ProjectId = taskRequiest.ProjectId,
                CreatedAt = taskRequiest.CreatedAt,
                ParentTaskId = taskRequiest.ParentTaskId,
                Number = taskRequiest.Number,
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
