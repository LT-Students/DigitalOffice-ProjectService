using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
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

        private void HandleInternalPropertyValidation(Operation<EditTaskRequest> requestedOperation, CustomContext context)
        {
            #region local functions

            void AddСorrectPaths(List<string> paths)
            {
                if (paths.FirstOrDefault(p => p.EndsWith(requestedOperation.path[1..], StringComparison.OrdinalIgnoreCase)) == null)
                {
                    context.AddFailure(requestedOperation.path, $"This path {requestedOperation.path} is not available");
                }
            }

            void AddСorrectOperations(
                string propertyName,
                List<OperationType> types)
            {
                if (requestedOperation.path.EndsWith(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !types.Contains(requestedOperation.OperationType))
                {
                    context.AddFailure(propertyName, $"This operation {requestedOperation.OperationType} is prohibited for {propertyName}");
                }
            }

            void AddFailureForPropertyIf(
                string propertyName,
                Func<OperationType, bool> type,
                Dictionary<Func<Operation<EditTaskRequest>, bool>, string> predicates)
            {
                if (!requestedOperation.path.EndsWith(propertyName, StringComparison.OrdinalIgnoreCase)
                    || !type(requestedOperation.OperationType))
                {
                    return;
                }

                foreach (var validateDelegate in predicates)
                {
                    if (!validateDelegate.Key(requestedOperation))
                    {
                        context.AddFailure(propertyName, validateDelegate.Value);
                    }
                }
            }

            #endregion

            #region paths

            AddСorrectPaths(
                new List<string>
                {
                    nameof(EditTaskRequest.Name),
                    nameof(EditTaskRequest.Description),
                    nameof(EditTaskRequest.AssignedTo),
                    nameof(EditTaskRequest.PriorityId),
                    nameof(EditTaskRequest.StatusId),
                    nameof(EditTaskRequest.TypeId),
                    nameof(EditTaskRequest.PlannedMinutes)
                });

            AddСorrectOperations(nameof(EditTaskRequest.Name), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditTaskRequest.Description), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditTaskRequest.AssignedTo), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditTaskRequest.PriorityId), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditTaskRequest.StatusId), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditTaskRequest.TypeId), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditTaskRequest.PlannedMinutes), new List<OperationType> { OperationType.Replace });

            #endregion

            #region firstname

            AddFailureForPropertyIf(
                nameof(EditTaskRequest.Name),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "Name is empty." },
                    { x => x.value.ToString().Length < 150, "Name is too long" }
                });

            #endregion

            #region description

            AddFailureForPropertyIf(
                nameof(EditTaskRequest.Description),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    {
                        x =>
                          {
                              if (string.IsNullOrEmpty(x.ToString()))
                              {
                                  return true;
                              }

                              return x.value.ToString().Length < 150;
                          },
                        "Name is too long"
                    }
                });

            #endregion

            #region assignedto

            AddFailureForPropertyIf(
                nameof(EditTaskRequest.AssignedTo),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid _), "Incorrect format of AssignedTo." },
                    { x => _userRepository.AreExist(Guid.Parse(x.value.ToString())), "The user must to in the project." }
                });

            #endregion

            #region priorityid

            AddFailureForPropertyIf(
                nameof(EditTaskRequest.PriorityId),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid _), "Incorrect format of PriorityId." },
                    { x => _taskPropertyRepository.AreExist(Guid.Parse(x.value.ToString()), TaskPropertyType.Priority), "The priority must exist." }
                });

            #endregion

            #region statusid

            AddFailureForPropertyIf(
                nameof(EditTaskRequest.StatusId),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid _), "Incorrect format of StatusId." },
                    { x => _taskPropertyRepository.AreExist(Guid.Parse(x.value.ToString()),
                        TaskPropertyType.Status), "The status must exist." }
                });

            #endregion

            #region typeid

            AddFailureForPropertyIf(
                nameof(EditTaskRequest.TypeId),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid _), "Incorrect format of TypeId." },
                    { x => _taskPropertyRepository.AreExist(Guid.Parse(x.value.ToString()), TaskPropertyType.Type), "The type must exist." }
                });

            #endregion

            #region PlannedMinutes

            AddFailureForPropertyIf(
                nameof(EditTaskRequest.PlannedMinutes),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    { x => int.TryParse(x.value.ToString(), out int minutes) && minutes > 0, "Incorrect format of PlannedMinutes." }
                });

            #endregion
        }

        public EditTaskValidator(
            ITaskPropertyRepository taskPropertyRepository,
            IUserRepository userRepository)
        {
            _taskPropertyRepository = taskPropertyRepository;
            _userRepository = userRepository;

            RuleForEach(x => x.Operations)
               .Custom(HandleInternalPropertyValidation);
        }
    }
}
