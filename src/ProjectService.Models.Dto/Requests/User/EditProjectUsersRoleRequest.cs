using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests.User
{
  public record EditProjectUsersRoleRequest
  {
    public ProjectUserRoleType Role { get; set; }
    [Required]
    public List<Guid> UsersIds { get; set; }
  }
}
