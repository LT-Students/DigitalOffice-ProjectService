using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateImageValidator : AbstractValidator<List<CreateImageRequest>>, ICreateImageValidator
    {
        public CreateImageValidator()
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
