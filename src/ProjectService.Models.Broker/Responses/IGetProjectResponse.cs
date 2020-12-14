using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetProjectResponse
    {
        Guid Id { get; }
        bool IsActive { get; }

        static object CreateObj(Guid projectId, bool isActive)
        {
            return new
            {
                Id = projectId,
                IsActive = isActive
            };
        }
    }
}
