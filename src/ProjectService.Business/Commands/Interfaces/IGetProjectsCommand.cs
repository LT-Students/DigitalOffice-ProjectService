using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    public interface IGetProjectsCommand
    {
        IEnumerable<Project> Execute(bool showNotActive);
    }
}
