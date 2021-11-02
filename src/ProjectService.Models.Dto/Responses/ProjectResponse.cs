using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Responses
{
  public class ProjectResponse
    {
        public ProjectInfo Project { get; set; }
        public IEnumerable<UserInfo> Users { get; set; }
        public IEnumerable<FileInfo> Files { get; set; }
        public IEnumerable<ImageInfo> Images { get; set; }
    }
}
