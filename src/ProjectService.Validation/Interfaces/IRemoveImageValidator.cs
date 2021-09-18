using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.ProjectService.Validation.Interfaces
{
  [AutoInject]
  public interface IRemoveImageValidator : IValidator<List<Guid>>
  {
  }
}
