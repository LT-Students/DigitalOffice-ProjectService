using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Helpers;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Db
{
    public class DbTaskMapper : IDbTaskMapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbTaskMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

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
                ProjectId = taskRequest.ProjectId,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
                ParentId = taskRequest.ParentId,
                PriorityId = taskRequest.PriorityId,
                StatusId = taskRequest.StatusId,
                TypeId = taskRequest.TypeId,
                Number = TaskNumberHelper.GetProjectTaskNumber(taskRequest.ProjectId)
            };
        }
    }
}
