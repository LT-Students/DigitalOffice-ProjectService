using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Validation.Image.Interfaces
{
  [AutoInject]
  public interface IRemoveImageValidator : IValidator<RemoveImageRequest>
  {
  }
}
