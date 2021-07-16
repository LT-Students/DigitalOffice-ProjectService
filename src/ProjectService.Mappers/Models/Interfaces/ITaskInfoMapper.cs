using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface ITaskInfoMapper
    {
        TaskInfo Map(DbTask dbTask, UserData assignedUser, UserData author);
    }
}
