using FluentValidation;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation.Image
{
  public class CreateImagesRequestValidator : AbstractValidator<CreateImagesRequest>, ICreateImagesRequestValidator
  {
    public CreateImagesRequestValidator(
      IImageValidator imageValidator,
      IProjectRepository projectRepository)
    {
      RuleFor(request => request.Images)
        .Cascade(CascadeMode.Stop)
        .NotNull().WithMessage("List of images must not be null.")
        .NotEmpty().WithMessage("List of images must not be empty.")
        .ForEach(image =>
        {
          image
          .Cascade(CascadeMode.Stop)
          .NotNull().WithMessage("Image must not be null.")
          .SetValidator(imageValidator);
        });

      RuleFor(request => request.ProjectId)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("Project id must not be empty.")
        .MustAsync(async (x, _) => await projectRepository.DoesExistAsync(x))
        .WithMessage("Invalid project id.");
    }
  }
}
