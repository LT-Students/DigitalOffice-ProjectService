using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.ProjectService.Validation.Interfaces
{
    [AutoInject]
    public interface IEditTaskValidator : IValidator<JsonPatchDocument<EditTaskRequest>>
    {
    }
}