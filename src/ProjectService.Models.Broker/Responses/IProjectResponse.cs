using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IProjectResponse
    {
        Guid Id { get; }
        string Name { get; }
        bool IsActive { get; }

        static object CreateObj(Guid id, string name, bool isActive)
        {
            return new
            {
                Id = id,
                Name = name,
                IsActive = isActive
            };
        }
    }
}
