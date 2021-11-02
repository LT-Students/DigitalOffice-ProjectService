using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Validation.File.Interfaces
{
  [AutoInject]
  public interface IFileContentValidator : IValidator<FileContent>
  {
  }
}
