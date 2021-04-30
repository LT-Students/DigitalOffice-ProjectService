using System;

namespace LT.DigitalOffice.ProjectService.Models.Broker.Requests
{
    public interface IGetUserProjectsRequest
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
