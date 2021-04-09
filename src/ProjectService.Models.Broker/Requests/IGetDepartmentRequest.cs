using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetDepartmentDataEndpoint))]
    public interface IGetDepartmentRequest
    {
        Guid DepartmentId { get; }

        static object CreateObj(Guid departmentId)
        {
            return new
            {
                DepartmentId = departmentId
            };
        }
    }
}
