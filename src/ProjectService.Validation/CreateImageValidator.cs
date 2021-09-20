using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class CreateImageValidator : AbstractValidator<CreateImageRequest>, ICreateImageValidator
  {
    private readonly List<string> imageFormats = new()
    {
      ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tga"
    };

    public CreateImageValidator()
    {
      RuleFor(images => images)
        .NotNull().WithMessage("List must not be null.")
        .NotEmpty().WithMessage("List must not be empty.");

      RuleFor(images => images.EntityId)
        .NotEmpty().WithMessage("Image's Id must not be empty.");

      RuleForEach(images => images.Images)
        .Must(images => !string.IsNullOrEmpty(images.Content))
        .WithMessage("Content can't be empty.")
        .Must(images => imageFormats.Contains(images.Extension))
        .WithMessage("Wrong extension.")
        .Must(images => images.Name.Length < 150)
        .WithMessage("Name's length must be less than 150 letters.")
        .Must(images =>
        {
          try
          {
            var byteString = new Span<byte>(new byte[images.Content.Length]);
            return Convert.TryFromBase64String(images.Content, byteString, out _);
          }
          catch
          {
            return false;
          }
        }).WithMessage("Wrong image content.");
    }
  }
}
