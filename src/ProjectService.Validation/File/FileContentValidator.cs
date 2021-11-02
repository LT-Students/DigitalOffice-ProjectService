using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Validation.File.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.File.CreateFileValidator
{
  public class FileContentValidator : AbstractValidator<FileContent>, IFileContentValidator
  {
    public FileContentValidator(
      IImageContentValidator contentValidator)
    {
      RuleFor(i => i.Content)
        .SetValidator(contentValidator)
        .WithMessage("Incorrect file content.");
    }
  }
}
