using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbProjectUserMapper
  {
    DbProjectUser Map(ProjectUserRequest request);
  }
}
