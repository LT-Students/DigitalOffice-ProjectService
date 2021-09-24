using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;

namespace LT.DigitalOffice.ProjectService.Validation
{
  public class ImageContentValidator : AbstractValidator<ImageContent>, IImageContentValidator
  {
    private readonly List<string> imageFormats = new()
    {
      ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tga"
    };

    public ImageContentValidator()
    {
      RuleFor(images => images)
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
            return Convert.TryFromBase64String(images.Content, new Span<byte>(new byte[images.Content.Length]), out _);
          }
          catch
          {
            return false;
          }
        }).WithMessage("Wrong image content.");
    }
  }
}
