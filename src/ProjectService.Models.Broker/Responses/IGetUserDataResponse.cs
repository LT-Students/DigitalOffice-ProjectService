using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUserDataResponse
    {
        Guid Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string MiddleName { get; }
        bool IsActive { get; }

        static object CreateObj(Guid id, string firstName, string middleName, string lastName, bool isActive)
        {
            return new
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                IsActive = isActive
            };
        }
    }
}
