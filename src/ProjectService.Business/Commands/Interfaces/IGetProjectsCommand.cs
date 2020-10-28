using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    public interface IGetProjectsCommand
    {
        IEnumerable<ProjectResponse> Execute(bool showNotActive);
    }
}
