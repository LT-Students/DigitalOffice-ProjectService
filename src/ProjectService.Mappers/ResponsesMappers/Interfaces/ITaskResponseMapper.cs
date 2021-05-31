using System.Collections.Generic;
using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces
{
    [AutoInject]
    public interface ITaskResponseMapper
    {
        public TaskResponse Map(
            DbTask dbTask,
            UserData authorUserData, 
            UserData parentAssignedUserData,
            UserData parentAuthorAssignedUserData, 
            string departmentName,
            UserData assignedUserData,
            ICollection<TaskInfo> subtasksInfo);
    }
}
