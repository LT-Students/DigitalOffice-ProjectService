using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
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
