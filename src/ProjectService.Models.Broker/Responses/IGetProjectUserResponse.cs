using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetProjectUserResponse
    {
        Guid Id { get; }
        bool IsActive { get; }

        static object CreateObj(Guid projectUserId, bool isActive)
        {
            return new
            {
                Id = projectUserId,
                IsActive = isActive
            };
        }
    }
}
