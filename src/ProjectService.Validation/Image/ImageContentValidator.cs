using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.Image
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
