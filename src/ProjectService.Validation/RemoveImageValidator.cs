using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class RemoveImageValidator : AbstractValidator<RemoveImageRequest>, IRemoveImageValidator
  {
    public RemoveImageValidator()
    {
      RuleFor(list => list.ImagesIds)
        .NotNull().WithMessage("List must not be null.")
        .NotEmpty().WithMessage("List must not be empty.")
        .ForEach(x => x.NotEmpty().WithMessage("Image's Id must not be empty."));
    }
  }
}
