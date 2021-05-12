using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;

namespace LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IProjectUserInfoMapper
    {
        ProjectUserInfo Map(UserData userData, DbProjectUser dbProjectUser);
    }
}
