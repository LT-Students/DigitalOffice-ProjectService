using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;

namespace LT.DigitalOffice.ProjectService.Validation.Interfaces
{
    [AutoInject]
    public interface IProjectUserValidator : IValidator<ProjectUserRequest>
    {
    }
}
