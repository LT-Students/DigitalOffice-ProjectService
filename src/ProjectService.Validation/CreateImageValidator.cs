using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class CreateImageValidator : AbstractValidator<CreateImageRequest>, ICreateImageValidator
  {
    public CreateImageValidator(
      IImageValidator imageValidator)
    {
      List<string> errors = new();

      RuleFor(images => images)
        .NotNull().WithMessage("List must not be null.")
        .NotEmpty().WithMessage("List must not be empty.");

      RuleFor(images => images.ProjectId)
        .NotEmpty().WithMessage("Image's Id must not be empty.");

      RuleForEach(images => images.Images)
        .SetValidator(imageValidator)
        .WithMessage("Incorrect image.");
    }
  }
}
