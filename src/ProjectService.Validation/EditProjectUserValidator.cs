using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class EditProjectUserValidator : AbstractValidator<EditProjectUserRequest>
    {
        private static List<string> Paths
            => new List<string> { NamePath, ShortNamePath, DescriptionPath, ClosedReasonPath, IsActivePath };

        private static string NamePath => $"/{nameof(DbProject.Name)}";
        private static string ShortNamePath => $"/{nameof(DbProject.ShortName)}";
        private static string DescriptionPath => $"/{nameof(DbProject.Description)}";
        private static string ClosedReasonPath => $"/{nameof(DbProject.ClosedReason)}";
        private static string IsActivePath => $"/{nameof(DbProject.IsActive)}";

        Func<JsonPatchDocument<DbProjectUser>, string, Operation> GetOperationByPath =>
            (x, path) => x.Operations.FirstOrDefault(x => x.path == path);

        public EditProjectUserValidator([FromServices] IProjectRepository repository)
        {
            RuleFor(x => x.ProjectUserId)
                .NotEmpty()
                .WithMessage("Request must have a project Id");

            RuleFor(project => project.Name)
                    .NotEmpty()
                    .WithMessage("Project must have a name.")
                    .MaximumLength(80)
                    .WithMessage("Project name is too long.");

            RuleFor(project => project.Description)
                .MaximumLength(500)
                .WithMessage("Project description is too long.");
        }
    }
}