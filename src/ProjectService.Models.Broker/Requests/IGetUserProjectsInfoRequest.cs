using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetUserProjectsInfoRequest
    {
        Guid UserId { get; }

        static object CreateObj(Guid userId)
        {
            return new
            {
                UserId = userId
            };
        }
    }
}
