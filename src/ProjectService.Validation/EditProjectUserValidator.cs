using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class EditProjectUserValidator : AbstractValidator<EditProjectUserRequest>
    {
        private static List<string> Paths
            => new List<string> { NamePath, ShortNamePath, DescriptionPath };

        public static string NamePath => $"/{nameof(DbProject.Name)}";
        public static string ShortNamePath => $"/{nameof(DbProject.ShortName)}";
        public static string DescriptionPath => $"/{nameof(DbProject.Description)}";

        Func<JsonPatchDocument<DbProject>, string, Operation> GetOperationByPath =>
            (x, path) => x.Operations.FirstOrDefault(x => x.path == path);

        public EditProjectValidator()
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
                        .UniqueOperationWithAllowedOp(DescriptionPath, "add", "replace", "remove");

                        When(x => GetOperationByPath(x.Patch, DescriptionPath).op != "remove", () =>
                        {
                            RuleFor(x => (string)GetOperationByPath(x.Patch, DescriptionPath).value)
                            .MaximumLength(500)
                            .WithMessage($"{DescriptionPath} is too long.");
                        });
                    });
                });
        }
    }
}