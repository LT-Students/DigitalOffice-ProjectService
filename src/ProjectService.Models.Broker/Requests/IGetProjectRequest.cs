using System;

namespace LT.DigitalOffice.ProjectService.Models.Broker.Requests
{
    public interface IGetProjectRequest
    {
        Guid Id { get; }

        static object CreateObj(Guid id)
        {
            return new
            {
                Id = id
            };
        }
    }
}
