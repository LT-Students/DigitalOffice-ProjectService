using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces
{
    [AutoInject]
    public interface IDepartmentInfoMapper
    {
        DepartmentInfo Map(IGetDepartmentResponse departmentResponse);
    }
}
