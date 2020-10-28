using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetDepartmentResponse
    {
        Guid Id { get; }
        string Name { get; }
    }
}
