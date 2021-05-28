using LT.DigitalOffice.Broker.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUserProjectsInfoResponse
    {
        List<ProjectShortInfo> Projects { get; }

        static object CreateObj(List<ProjectShortInfo> projects)
        {
            return new
            {
                Projects = projects
            };
        }
    }
}
