﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.Project
{
  public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>, ICreateProjectRequestValidator
  {
    public CreateProjectRequestValidator(
      IProjectRepository projectRepository,
      IUserService userService,
      IImageValidator imageValidator)
    {
      CascadeMode = CascadeMode.Stop;

      RuleFor(project => project.Name.Trim())
        .MaximumLength(150).WithMessage("Project name is too long.");

      RuleFor(project => project.ShortName)
        .MaximumLength(40).WithMessage("Project short name is too long.");

      RuleFor(project => project)
        .MustAsync(async (project, _) => !await projectRepository.DoesProjectNamesExistAsync(project.Name, project.ShortName))
        .WithMessage(project => "Project's name and short name must be unique.");

      RuleFor(project => project.Status)
        .IsInEnum();

      When(project => !string.IsNullOrEmpty(project.ShortDescription?.Trim()), () =>
      {
        RuleFor(project => project.ShortDescription)
          .MaximumLength(300)
          .WithMessage("Project short description is too long.");
      });

      When(project => !string.IsNullOrEmpty(project.Customer?.Trim()), () =>
      {
        RuleFor(project => project.ShortDescription)
          .MaximumLength(150)
          .WithMessage("Project customer is too long.");
      });

      When(project => !project.Status.Equals(ProjectStatusType.Active), () =>
      {
        RuleFor(project => project.EndDateUtc)
          .Must(endDateUtc => endDateUtc.HasValue)
          .WithMessage("EndDateUtc can't be null if project is not active.");
      });

      When(project => project.DepartmentId.HasValue, () =>
      {
        RuleFor(project => project.DepartmentId)
          .Cascade(CascadeMode.Stop)
          .Must(departmentId => departmentId != Guid.Empty)
          .WithMessage("Wrong type of department Id.");
      });

      When(project => project.Users.Any(), () =>
      {
        RuleForEach(project => project.Users)
          .ChildRules(user =>
          {
            user.RuleFor(user => user.UserId)
              .NotEmpty().WithMessage("Wrong type of user Id.");

            user.RuleFor(user => user.Role)
              .IsInEnum();
          });

        RuleFor(project => project.Users)
          .Cascade(CascadeMode.Stop)
          .Must(p => p.Select(pu => pu.UserId).Distinct().Count() == p.Count())
          .WithMessage("User cannot be added to the project twice.")
          .MustAsync(async (projectUsers, cancellation) =>
            (await userService.CheckUsersExistenceAsync(projectUsers.Select(user => user.UserId).ToList())).Count() == projectUsers.Count())
          .WithMessage("Some users does not exist.");
      });

      When(project => project.ProjectImages.Any(), () =>
      {
        RuleForEach(project => project.ProjectImages)
          .SetValidator(imageValidator);
      });
    }
  }
}
