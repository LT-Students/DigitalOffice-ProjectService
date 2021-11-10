using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateProjectRequest
  {
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public Guid? DepartmentId { get; set; }
    public ProjectStatusType Status { get; set; }
    public List<CreateUserRequest> Users { get; set; }
    public List<ImageContent> ProjectImages { get; set; }
    public List<FileContent> Files { get; set; }
  }
}
