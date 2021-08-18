using FluentValidation;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class CreateProjectValidator : AbstractValidator<ProjectRequest>, ICreateProjectValidator
    {
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
        }
    }
}