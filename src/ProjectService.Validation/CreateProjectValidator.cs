using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectRequest>, ICreateProjectValidator
    {
        private readonly List<string> imageFormats = new()
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp",
            ".gif",
            ".tga"
        };

        public CreateProjectValidator()
        {
            RuleFor(project => project.Name)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("Project name is too long.");

            RuleFor(project => project.Status)
                .IsInEnum();

            When(project => !string.IsNullOrEmpty(project.ShortName?.Trim()), () =>
            {
                RuleFor(project => project.ShortName)
                    .MaximumLength(30)
                    .WithMessage("Project short name is too long.");
            });

            When(project => project.Users != null && project.Users.Any(), () =>
            {
                RuleForEach(project => project.Users).ChildRules(user =>
                {
                    user.RuleFor(user => user.UserId)
                        .NotEmpty();

                    user.RuleFor(user => user.Role)
                        .IsInEnum();
                });
            });

            When(project => project.ProjectImages != null && project.ProjectImages.Any(), () =>
            {
                RuleForEach(project => project.ProjectImages)
                    .Must(x => !string.IsNullOrEmpty(x.Content))
                    .WithMessage("Content can't be empty")
                    .Must(x => imageFormats.Contains(x.Extension))
                    .WithMessage("Wrong extension")
                    .Must(images => images.Name.Length < 150)
                    .WithMessage("Name's length must be less than 150 letters");
            });

            When(project => !string.IsNullOrEmpty(project.ShortDescription?.Trim()), () =>
            {
                RuleFor(project => project.ShortDescription)
                    .MaximumLength(300)
                    .WithMessage("Project short description is too long.");
            });

            When(project => !string.IsNullOrEmpty(project.Description?.Trim()), () =>
            {
                RuleFor(project => project.Description)
                    .MaximumLength(300)
                    .WithMessage("Project description is too long.");
            });

            When(
                news => news.DepartmentId.HasValue,
                () =>
                    RuleFor(news => news.DepartmentId)
                        .Must(DepartmentId => DepartmentId != Guid.Empty)
                        .WithMessage("Wrong type of department Id."));
        }
    }
}