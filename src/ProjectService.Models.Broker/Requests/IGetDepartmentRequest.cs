using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetDepartmentEndpoint))]
    public interface IGetDepartmentRequest
    {
        Guid? UserId { get; }
        Guid? DepartmentId { get; }

        static object CreateObj(Guid? userId, Guid? departmentId)
        {
            return new
            {
                UserId = userId,
                DepartmentId = departmentId
            };
        }
    }
}
