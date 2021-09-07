using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class RemoveImageValidator : AbstractValidator<List<RemoveImageRequest>>, IRemoveImageValidator
    {
        public RemoveImageValidator()
        {
            RuleFor(pu => pu)
                .NotNull()
                .WithMessage("List must not be null");

            RuleFor(pu => pu)
                .NotEmpty()
                .WithMessage("List must not be empty");
        }
    }
}
