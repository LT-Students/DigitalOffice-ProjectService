using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.Image
{
  public class RemoveImagesRequestValidator : AbstractValidator<RemoveImageRequest>, IRemoveImagesRequestValidator
  {
    public RemoveImagesRequestValidator()
    {
      RuleFor(list => list.ImagesIds)
        .NotNull().WithMessage("List must not be null.")
        .NotEmpty().WithMessage("List must not be empty.")
        .ForEach(x =>
          x.NotEmpty().WithMessage("Image Id must not be empty."));
    }
  }
}
