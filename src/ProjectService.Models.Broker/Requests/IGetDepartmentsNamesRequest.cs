using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetDepartmentsNamesEndpoint))]
    public interface IGetDepartmentsNamesRequest
    {
        IList<Guid> Ids { get; }

        static object CreateObj(IList<Guid> ids)
        {
            return new
            {
                Ids = ids
            };
        }
    }
}
