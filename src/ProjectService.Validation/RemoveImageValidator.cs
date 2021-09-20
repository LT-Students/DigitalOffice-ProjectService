using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class RemoveImageValidator : AbstractValidator<List<RemoveImageRequest>>, IRemoveImageValidator
    {
        public RemoveImageValidator()
        {
            RuleFor(list => list)
                .NotNull().WithMessage("List must not be null")
                .NotEmpty().WithMessage("List must not be empty");

            RuleForEach(list => list)
                .ChildRules(list =>
                {
                    list.RuleFor(list => list.ImageId)
                        .NotEmpty().WithMessage("Image's Id must not be empty");
                });
        }
    }
}
