using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetDepartmentResponse
    {
        Guid Id { get; }
        string Name { get; }

        static object CreateObj(Guid id, string name)
        {
            return new
            {
                Id = id,
                Name = name
            };
        }
    }
}