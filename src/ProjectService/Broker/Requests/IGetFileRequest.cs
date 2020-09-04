using System;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// Represents request for GetFileConsumer in MassTransit logic.
    /// </summary>
    public interface IGetFileRequest
    {
        Guid FileId { get; }
    }
}