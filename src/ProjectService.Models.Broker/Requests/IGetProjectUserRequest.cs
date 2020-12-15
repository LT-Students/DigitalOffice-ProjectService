using System;

namespace LT.DigitalOffice.Broker.Requests
{
    public interface IGetProjectUserRequest
    {
        Guid ProjectId { get; }
        Guid UserId { get; }

        static object CreateObj(Guid projectId, Guid userId)
        {
            return new
            {
                ProjectId = projectId,
                UserId = userId
            };
        }
    }
}
