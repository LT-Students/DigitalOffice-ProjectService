using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetProjectUserRequest
    {
        Guid ProjectId { get; }
        Guid UserId { get; }
    }
}
