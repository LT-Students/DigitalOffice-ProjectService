using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IProjectResponse
    {
        Guid Id { get; }
        string Name { get; }
        int ProjectStatus { get; set; }

        static object CreateObj(Guid id, string name, int projectStatus)
        {
            return new
            {
                Id = id,
                Name = name,
                ProjectStatus = projectStatus
            };
        }
    }
}
