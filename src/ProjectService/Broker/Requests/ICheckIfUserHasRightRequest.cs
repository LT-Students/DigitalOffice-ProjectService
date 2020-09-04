using System;

namespace LT.DigitalOffice.Broker.Requests
{
    /// <summary>
    /// Represents request for CheckIfUserHasRightConsumer in MassTransit logic.
    /// </summary>
    public interface ICheckIfUserHasRightRequest
    {
        int RightId { get; }
        Guid UserId { get; }
    }
}