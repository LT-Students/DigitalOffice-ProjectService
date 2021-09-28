using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Validation.Interfaces
{
  [AutoInject]
  public interface IEditTaskPropertyValidator : IValidator<JsonPatchDocument<TaskProperty>>
  {
  }
}
