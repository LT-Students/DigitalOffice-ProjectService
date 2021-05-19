using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetDepartmentResponse
    {
        Guid Id { get; }
        string Name { get; }
        Guid? DirectorUserId { get; }

        static object CreateObj(Guid id, string name, Guid directorUserId)
        {
            return new
            {
                Id = id,
                Name = name,
                DirectorUserId = directorUserId
            };
        }
    }
}