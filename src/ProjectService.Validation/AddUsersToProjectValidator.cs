using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class AddUsersToProjectValidator : AbstractValidator<AddUsersToProjectRequest>, IAddUsersToProjectValidator
  {
    public AddUsersToProjectValidator(
      IProjectUserValidator projectUserValidator,
      IProjectRepository projectRepository)
    {
      CascadeMode = CascadeMode.Stop;

      RuleFor(projectUser => projectUser.ProjectId)
        .NotEmpty()
        .WithMessage("Request must have a project Id")
        .Must(projectRepository.IsExist)
        .WithMessage("This project id does not exist")
        .DependentRules(() =>
        {
          RuleFor(projectUser => projectUser.Users)
            .Must(user => user != null && user.Any())
            .WithMessage("The request must contain users");

          RuleForEach(projectUser => projectUser.Users)
            .SetValidator(projectUserValidator);
        });
    }
  }
}
