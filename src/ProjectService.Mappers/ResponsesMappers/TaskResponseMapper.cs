using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers
{
    public class TaskResponseMapper : ITaskResponseMapper
    {
        public TaskResponse Map(DbTask dbTask)
        {
            return new TaskResponse()
            {
                Id = dbTask.Id,
                TypeId = dbTask.TypeId,
                AuthorId = dbTask.AuthorId,
                StatusId = dbTask.StatusId,
                ParentId = dbTask.ParentId,
                ProjectId = dbTask.ProjectId,
                PriorityId = dbTask.PriorityId,
                AssignedTo = dbTask.AssignedTo,
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