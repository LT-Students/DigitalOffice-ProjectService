using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;


namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces
{
    public interface ICreateRoleRequestMapper : IMapper<DbRole, CreateRoleRequest>, IMapper<CreateRoleRequest, DbRole> { }
}
