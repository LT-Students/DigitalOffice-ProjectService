using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class TaskInfoMapper : ITaskInfoMapper
    {
        private readonly IUserTaskInfoMapper _userMapper;

        public TaskInfoMapper(IUserTaskInfoMapper userMapper)
        {
            _userMapper = userMapper;
        }

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
                CreatedAtUtc = dbTask.CreatedAtUtc,
                StatusName = dbTask.Status?.Name,
                Description = dbTask.Description,
                PriorityName = dbTask.Priority?.Name,
                PlannedMinutes = dbTask.PlannedMinutes,
                CreatedBy = _userMapper.Map(author),
                Project = dbTask.Project != null
                    ? new ProjectTaskInfo
                    {
                        Id = dbTask.ProjectId,
                        ShortName = dbTask.Project.ShortName
                    }
                    : null,
                AssignedTo = _userMapper.Map(assignedUser)
            };
        }
    }
}
