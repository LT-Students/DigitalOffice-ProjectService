using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// Represents request for GetFileConsumer in MassTransit logic.
    /// </summary>
    [AutoInjectRequest(nameof(RabbitMqConfig.GetFileEndpoint))]
    public interface IGetFileRequest
    {
        Guid FileId { get; }
    }
}