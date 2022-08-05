using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;

namespace LT.DigitalOffice.ProjectService.Validation.Project.Interfaces
{
  [AutoInject]
  public interface ICreateProjectRequestValidator : IValidator<CreateProjectRequest>
  {
  }
}
