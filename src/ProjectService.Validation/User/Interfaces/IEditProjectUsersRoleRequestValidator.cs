using System;
using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.ProjectService.Validation.User.Interfaces
{
  [AutoInject]
  public interface IEditProjectUsersRoleRequestValidator : IValidator<(Guid projectId, EditProjectUsersRoleRequest request)>
  {
  }
}
