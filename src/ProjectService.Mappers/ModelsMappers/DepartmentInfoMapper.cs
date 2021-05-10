using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class DepartmentInfoMapper : IDepartmentInfoMapper
    {
        public DepartmentInfo Map(IGetDepartmentResponse departmentResponse)
        {
            if (departmentResponse == null)
            {
                throw new ArgumentNullException(nameof(departmentResponse));
            }

            return new DepartmentInfo
            {
                Id = departmentResponse.Id,
                Name = departmentResponse.Name
            };
        }
    }
}
