using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Validation
{
    public class EditTaskValidator : AbstractValidator<JsonPatchDocument<EditTaskRequest>>, IEditTaskValidator
    {
        private readonly ITaskPropertyRepository _taskPropertyRepository;
        private readonly IUserRepository _userRepository;

        private static List<string> Paths
            => new() {Name, Description, AssignedTo, PriorityId, StatusId, TypeId, PlannedMinutes};

        public static string Name => $"/{nameof(EditTaskRequest.Name)}";
        public static string Description => $"/{nameof(EditTaskRequest.Description)}";
        public static string AssignedTo => $"/{nameof(EditTaskRequest.AssignedTo)}";
        public static string PriorityId => $"/{nameof(EditTaskRequest.PriorityId)}";
        public static string StatusId => $"/{nameof(EditTaskRequest.StatusId)}";
        public static string TypeId => $"/{nameof(EditTaskRequest.TypeId)}";
        public static string PlannedMinutes => $"/{nameof(EditTaskRequest.PlannedMinutes)}";

        Func<JsonPatchDocument<EditTaskRequest>, string, Operation> GetOperationByPath =>
            (x, path) =>
                x.Operations.FirstOrDefault(x => 
                    string.Equals(x.path, path, StringComparison.OrdinalIgnoreCase));

        public EditTaskValidator(
            ITaskPropertyRepository taskPropertyRepository,
            IUserRepository userRepository)
        {
            _taskPropertyRepository = taskPropertyRepository;
            _userRepository = userRepository;

            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Operations)
                .Must(x =>
                    x.Select(x => x.path)
                        .Distinct().Count() == x.Count())
                .WithMessage("You don't have to change the same field of Task multiple times.")
                .Must(x => x.Any())
                .WithMessage("You don't have changes.")
                .ForEach(y => y
                    .Must(x => Paths.Any(cur => string.Equals(
                        cur,
                        x.path,
                        StringComparison.OrdinalIgnoreCase)))
                    .WithMessage(
                        $"Document contains invalid path. Only such paths are allowed: {Paths.Aggregate((x, y) => x + ", " + y)}")
                )
                .DependentRules(() =>
                {
                    When(x => GetOperationByPath(x, Name) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(Name, "replace", "add", "remove");

                        RuleFor(x => (string) GetOperationByPath(x, Name).value)
                            .NotEmpty()
                            .MaximumLength(150)
                            .WithMessage("Name is too long");
                    });

                    When(x => GetOperationByPath(x, Description) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(Description, "replace", "add", "remove");

                        RuleFor(x => GetOperationByPath(x, Description).value)
                            .NotNull();
                    });

                    When(x => GetOperationByPath(x, AssignedTo) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(AssignedTo, "replace", "add", "remove");

                        RuleFor(x => GetOperationByPath(x, AssignedTo).value)
                            .NotEmpty()
                            .Must(o => Guid.TryParse(o.ToString(), out Guid _))
                            .Must(o => _userRepository.AreExist((Guid) o));
                    });

                    When(x => GetOperationByPath(x, PriorityId) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(PriorityId, "replace");

                        RuleFor(x => GetOperationByPath(x, PriorityId).value)
                            .NotEmpty()
                            .Must(o => Guid.TryParse(o.ToString(), out Guid _))
                            .Must(o => _taskPropertyRepository.AreExist((Guid) o));
                    });

                    When(x => GetOperationByPath(x, StatusId) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .NotEmpty()
                            .UniqueOperationWithAllowedOp(StatusId, "replace");

                        RuleFor(x => GetOperationByPath(x, StatusId).value)
                            .NotEmpty()
                            .Must(o => Guid.TryParse(o.ToString(), out Guid _))
                            .Must(o => _taskPropertyRepository.AreExist((Guid) o));
                    });

                    When(x => GetOperationByPath(x, TypeId) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .NotEmpty()
                            .UniqueOperationWithAllowedOp(StatusId, "replace");

                        RuleFor(x => GetOperationByPath(x, TypeId).value)
                            .NotEmpty()
                            .Must(o => Guid.TryParse(o.ToString(), out Guid _))
                            .Must(o => _taskPropertyRepository.AreExist((Guid) o));
                    });

                    When(x => GetOperationByPath(x, PlannedMinutes) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(PlannedMinutes, "replace", "add", "remove");

                        RuleFor(x => GetOperationByPath(x, PlannedMinutes).value)
                            .NotEmpty()
                            .Must(x => (int) x >= 0);
                    });
                });
        }
    }
}
