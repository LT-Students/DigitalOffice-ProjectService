using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.FindDepartmentsEndpoint))]
    public interface IFindDepartmentsRequest
    {
        string DepartmentName { get; }
        IList<Guid> DepartmentIds { get; }

        static object CreateObj(string departmentName, IList<Guid> departmentIds)
        {
            return new
            {
                DepartmentName = departmentName,
                DepartmentIds = departmentIds
            };
        }
    }
}
