using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IProjectsResponse
    {
        IList<Guid> ProjectsIds { get; }

        static object CreateObj(IList<Guid> projectsIds)
        {
            return new
            {
                ProjectsIds = projectsIds
            };
        }
    }
}
