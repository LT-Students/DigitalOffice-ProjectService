using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;

namespace LT.DigitalOffice.ProjectService.Validation.Interfaces
{
    [AutoInject]
    public interface IProjectUserRequestValidator : IValidator<ProjectUserRequest>
    {
    }
}
