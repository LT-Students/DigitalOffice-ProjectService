using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.Image
{
  public class ImageValidator : AbstractValidator<ImageContent>, IImageValidator
  {
    public ImageValidator(
      IImageContentValidator contentValidator,
      IImageExtensionValidator extensionValidator)
    {
      RuleFor(i => i.Content)
        .SetValidator(contentValidator);

      RuleFor(i => i.Extension)
        .SetValidator(extensionValidator);
    }
  }
}
