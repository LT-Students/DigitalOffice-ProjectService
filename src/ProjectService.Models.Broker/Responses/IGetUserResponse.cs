using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUserResponse
    {
        Guid Id { get; }
        string FirstName { get; }
        string MiddleName { get; }
        string LastName { get; set; }
    }
}
