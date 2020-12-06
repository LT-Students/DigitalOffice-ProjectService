using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class EditProjectValidator : AbstractValidator<EditProjectRequest>
    {
        private static List<string> Paths
            => new List<string> { NamePath, ShortNamePath, DescriptionPath, ClosedReasonPath, IsActivePath };

        private static string NamePath => $"/{nameof(DbProject.Name)}";
        private static string ShortNamePath => $"/{nameof(DbProject.ShortName)}";
        private static string DescriptionPath => $"/{nameof(DbProject.Description)}";
        private static string ClosedReasonPath => $"/{nameof(DbProject.ClosedReason)}";
        private static string IsActivePath => $"/{nameof(DbProject.IsActive)}";

        Func<JsonPatchDocument<DbProject>, string, Operation> GetOperationByPath =>
            (x, path) => x.Operations.FirstOrDefault(x => x.path == path);

        public EditProjectValidator([FromServices] IProjectRepository repository)
        {
            RuleFor(x => x.Patch.Operations)
                .Must(x => x.Select(x => x.path).Distinct().Count() == x.Count())
                .WithMessage("You don't have to change the same field of Project multiple times.")
                .Must(x => x.Any())
                .WithMessage("You don't have changes.")
                .ForEach(x => x
                .Must(x => Paths.Contains(x.path))
                .WithMessage($"Document contains invalid path. Only such paths are allowed: {Paths.Aggregate((x, y) => x + ", " + y)}")
                )
                .DependentRules(() =>
                {
                    When(x => GetOperationByPath(x.Patch, NamePath) != null, () =>
                    {
                        RuleFor(x => x.Patch.Operations)
                        .UniqueOperationWithAllowedOp(NamePath, "add", "replace");

                        RuleFor(x => (string)GetOperationByPath(x.Patch, NamePath).value)
                        .NotEmpty()
                        .WithMessage($"{NamePath} must not be empty.")
                        .MaximumLength(80)
                        .WithMessage($"{NamePath} is too long.");
                    });

                    When(x => GetOperationByPath(x.Patch, ShortNamePath) != null, () =>
                    {
                        RuleFor(x => x.Patch.Operations)
                        .UniqueOperationWithAllowedOp(ShortNamePath, "add", "replace", "remove");

                        When(x => GetOperationByPath(x.Patch, ShortNamePath).op != "remove", () =>
                        {
                            RuleFor(x => (string)GetOperationByPath(x.Patch, ShortNamePath).value)
                            .MaximumLength(32)
                            .WithMessage($"{ShortNamePath} is too long.");
                        });
                    });

                    When(x => GetOperationByPath(x.Patch, DescriptionPath) != null, () =>
                    {
                        RuleFor(x => x.Patch.Operations)
                        .UniqueOperationWithAllowedOp(DescriptionPath, "add", "replace");

                        When(x => GetOperationByPath(x.Patch, DescriptionPath).op != "remove", () =>
                        {
                            RuleFor(x => (string)GetOperationByPath(x.Patch, DescriptionPath).value)
                            .MaximumLength(500)
                            .WithMessage($"{DescriptionPath} is too long.");
                        });
                    });

                    When(x => GetOperationByPath(x.Patch, IsActivePath) != null, () =>
                    {
                        RuleFor(x => x.Patch.Operations)
                        .UniqueOperationWithAllowedOp(IsActivePath, "add", "replace");

                        When(x => (bool)GetOperationByPath(x.Patch, IsActivePath).value, () =>
                        {
                            // If new IsActive == true => reason for the closure should be null
                            RuleFor(x => x.Patch.Operations)
                            .UniqueOperationWithAllowedOp(ClosedReasonPath, "add", "replace", "remove");

                            When(x => GetOperationByPath(x.Patch, ClosedReasonPath).op != "remove", () =>
                            {
                                RuleFor(x => CastIfNotNull<ProjectClosedReason>(GetOperationByPath(x.Patch, ClosedReasonPath).value))
                                .Null()
                                .WithMessage($"If the project is open, then you need to update its {ClosedReasonPath} field to null.");
                            });
                        })
                        .Otherwise(() =>
                        {
                            RuleFor(x => GetOperationByPath(x.Patch, ClosedReasonPath))
                            .NotNull()
                            .DependentRules(() =>
                            {
                                // If new IsActive == false => reason for the closure should be
                                RuleFor(x => x.Patch.Operations)
                                .UniqueOperationWithAllowedOp(ClosedReasonPath, "add", "replace");

                                RuleFor(x => CastIfNotNull<ProjectClosedReason>(GetOperationByPath(x.Patch, ClosedReasonPath).value))
                                .NotNull()
                                .WithMessage($"If the project is closed, then you need to update its {ClosedReasonPath} field.")
                                .IsInEnum()
                                .WithMessage("There is no such reason for closing the project.");
                            });
                        });
                    })
                    .Otherwise(() =>
                    {
                        When(x => GetOperationByPath(x.Patch, ClosedReasonPath) != null, () =>
                        {
                            bool? isActive = true;

                            RuleFor(x => x.ProjectId)
                            .Must(x =>
                            {
                                isActive = repository.GetProject(x)?.IsActive;
                                return isActive != null;
                            })
                            .WithMessage("Project with this id is not exist.")
                            .DependentRules(() =>
                            {
                                if ((bool)isActive)
                                {
                                    RuleFor(x => x.Patch.Operations)
                                    .UniqueOperationWithAllowedOp(ClosedReasonPath, "add", "replace", "remove");

                                    // If actual IsActive == true => reason for closing should not change
                                    When(x => GetOperationByPath(x.Patch, ClosedReasonPath).op != "remove", () =>
                                    {
                                        RuleFor(x => GetOperationByPath(x.Patch, ClosedReasonPath))
                                        .Null()
                                        .WithMessage($"The project is open, you cannot provide a {ClosedReasonPath} without closing the project.");
                                    });
                                }
                                else
                                {
                                    RuleFor(x => x.Patch.Operations)
                                    .UniqueOperationWithAllowedOp(ClosedReasonPath, "add", "replace");

                                    // If actual IsActive == false => reason for closing can be replace
                                    RuleFor(x => CastIfNotNull<ProjectClosedReason>(GetOperationByPath(x.Patch, ClosedReasonPath).value))
                                    .NotNull()
                                    .WithMessage($"Project is closed, then you need to update its {ClosedReasonPath} field.")
                                    .IsInEnum()
                                    .WithMessage("There is no such reason for closing the project.");
                                };
                            });
                        });
                    });
                });
        }

        private T? CastIfNotNull<T>(object value) where T : struct
        {
            if (value == null)
            {
                return null;
            }

            return (T)value;
        }
    }
}