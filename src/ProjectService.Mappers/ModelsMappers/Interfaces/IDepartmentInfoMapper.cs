using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces
{
    [AutoInject]
    public interface IDepartmentInfoMapper
    {
        DepartmentInfo Map(IGetDepartmentResponse departmentResponse);
    }
}
