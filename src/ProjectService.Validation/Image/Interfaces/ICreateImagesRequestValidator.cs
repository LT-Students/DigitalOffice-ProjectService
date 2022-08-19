using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Image;

namespace LT.DigitalOffice.ProjectService.Validation.Image.Interfaces
{
  [AutoInject]
  public interface ICreateImagesRequestValidator : IValidator<CreateImagesRequest>
  {
  }
}
