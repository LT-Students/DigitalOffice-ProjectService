using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class RemoveImageValidator : AbstractValidator<List<Guid>>, IRemoveImageValidator
  {
    public RemoveImageValidator()
    {
      RuleFor(list => list)
        .NotNull().WithMessage("List must not be null.")
        .NotEmpty().WithMessage("List must not be empty.")
        .ForEach(x => x.NotEmpty().WithMessage("Image's Id must not be empty."));
    }
  }
}
