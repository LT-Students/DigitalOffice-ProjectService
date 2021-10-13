using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class ImageContentValidator : AbstractValidator<ImageContent>, IImageValidator
  {
    public ImageContentValidator(
      IImageContentValidator contentValidator,
      IImageExtensionValidator extensionValidator)
    {
      RuleFor(i => i.Content)
        .SetValidator(contentValidator)
        .WithMessage("Incorrect image content.");

      RuleFor(i => i.Extension)
        .SetValidator(extensionValidator)
        .WithMessage("Incorrect image extension.");
    }
  }
}
