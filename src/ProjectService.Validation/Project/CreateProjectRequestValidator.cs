using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using FluentValidation;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Validation.Project.Resources;

namespace LT.DigitalOffice.ProjectService.Validation.Project
{
  public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>, ICreateProjectRequestValidator
  {
    public CreateProjectRequestValidator(
      IProjectRepository projectRepository,
      IUserService userService,
      IImageValidator imageValidator)
    {
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

      CascadeMode = CascadeMode.Stop;

      RuleFor(project => project.Name.Trim())
        .MaximumLength(150).WithMessage(ProjectRequestValidationResource.NameLong)
        .MustAsync(async (name, _) => !await projectRepository.DoesNameExistAsync(name))
        .WithMessage(ProjectRequestValidationResource.NameExists);

      RuleFor(project => project.ShortName.Trim())
        .MaximumLength(40).WithMessage(ProjectRequestValidationResource.ShortNameLong)
        .MustAsync(async (shortName, _) => !await projectRepository.DoesShortNameExistAsync(shortName))
        .WithMessage(ProjectRequestValidationResource.ShortNameExists);

      RuleFor(project => project.Status)
        .IsInEnum();

      When(project => !string.IsNullOrEmpty(project.ShortDescription?.Trim()), () =>
      {
        RuleFor(project => project.ShortDescription)
          .MaximumLength(300)
          .WithMessage(ProjectRequestValidationResource.ShortDescriptionLong);
      });

      When(project => !string.IsNullOrEmpty(project.Customer?.Trim()), () =>
      {
        RuleFor(project => project.ShortDescription)
          .MaximumLength(150)
          .WithMessage(ProjectRequestValidationResource.CustomerLong);
      });

      When(project => !project.Status.Equals(ProjectStatusType.Active), () =>
      {
        RuleFor(project => project.EndDateUtc)
          .Must(endDateUtc => endDateUtc.HasValue)
          .WithMessage(ProjectRequestValidationResource.EndDateUtcIsNull);
      });

      When(project => project.DepartmentId.HasValue, () =>
      {
        RuleFor(project => project.DepartmentId)
          .Cascade(CascadeMode.Stop)
          .Must(departmentId => departmentId != Guid.Empty)
          .WithMessage(ProjectRequestValidationResource.DepartmentIdIsEmpty);
      });

      When(project => project.Users.Any(), () =>
      {
        RuleForEach(project => project.Users)
          .ChildRules(user =>
          {
            user.RuleFor(user => user.UserId)
              .NotEmpty().WithMessage(ProjectRequestValidationResource.UserIdIsEmpty);

            user.RuleFor(user => user.Role)
              .IsInEnum();
          });

        RuleFor(project => project.Users)
          .Cascade(CascadeMode.Stop)
          .Must(p => p.Select(pu => pu.UserId).Distinct().Count() == p.Count())
          .WithMessage(ProjectRequestValidationResource.AddUserTwice)
          .MustAsync(async (projectUsers, cancellation) =>
            (await userService.CheckUsersExistenceAsync(projectUsers.Select(user => user.UserId).ToList())).Count() == projectUsers.Count())
          .WithMessage(ProjectRequestValidationResource.UsersDoNotExist);
      });

      When(project => project.ProjectImages.Any(), () =>
      {
        RuleForEach(project => project.ProjectImages)
          .SetValidator(imageValidator);
      });
    }
  }
}
