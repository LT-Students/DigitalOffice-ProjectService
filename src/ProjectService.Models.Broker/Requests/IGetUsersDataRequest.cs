using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetUsersDataEndpoint))]
    public interface IGetUsersDataRequest
    {
        List<Guid> UserIds { get; }

        static object CreateObj(List<Guid> userIds)
        {
            return new
            {
                UserIds = userIds
            };
        }
    }
}
