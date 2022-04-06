using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
  public class ProjectResponse
  {
    public ProjectInfo Project { get; set; }
    public IEnumerable<UserInfo> Users { get; set; }
    public IEnumerable<FileAccess> Files { get; set; }
    public IEnumerable<ImageInfo> Images { get; set; }
  }
}
