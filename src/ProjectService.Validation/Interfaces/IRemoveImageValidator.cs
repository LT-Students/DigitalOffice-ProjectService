using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Validation.Interfaces
{
    [AutoInject]
    public interface IRemoveImageValidator : IValidator<List<Guid>>
    {
    }
}
