using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetProjectRequest
    {
        Guid ProjectId { get; }
    }
}
