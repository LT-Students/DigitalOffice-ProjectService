﻿using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class ProjectExpandedValidator : AbstractValidator<ProjectExpandedRequest>, IProjectExpandedValidator
    {
        public ProjectExpandedValidator()
        {
            RuleFor(project => project.Project.Name)
                .NotEmpty()
                .WithMessage("Project must have a name.")
                .MaximumLength(80)
                .WithMessage("Project name is too long.");

            When(project => project.Project.ShortName != null, () =>
            {
                RuleFor(project => project.Project.ShortName)
                .MaximumLength(32)
                .WithMessage("Project short name is too long.");
            });

            When(project => project.Project.Description != null, () =>
            {
                RuleFor(project => project.Project.Description)
                .MaximumLength(500)
                .WithMessage("Project description is too long.");
            });

            When(project => project.Users != null, () =>
            {
                RuleForEach(project => project.Users).SetValidator(new ProjectUserValidator());
            });
        }
    }
}