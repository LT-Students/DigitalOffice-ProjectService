using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
  public record CreateProjectRequest
  {
    [Required]
    public string Name { get; set; }
    [Required]
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    public string Customer { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public Guid? DepartmentId { get; set; }
    public ProjectStatusType Status { get; set; }

    [Required]
    public List<ImageContent> ProjectImages { get; set; }

    [Required]
    public List<UserRequest> Users { get; set; }
  }
}
