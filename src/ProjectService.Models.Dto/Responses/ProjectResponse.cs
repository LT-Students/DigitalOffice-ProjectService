using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
  public class ProjectResponse
  {
    public ProjectInfo Project { get; set; }
    public IEnumerable<ProjectUserInfo> Users { get; set; }
  }
}
