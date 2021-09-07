using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IProjectInfoMapper
    {
        ProjectInfo Map(DbProject dbProject, string departmentName);
    }
}
