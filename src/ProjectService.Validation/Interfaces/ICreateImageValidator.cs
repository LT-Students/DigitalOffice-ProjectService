using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Validation.Interfaces
{
    [AutoInject]
    public interface ICreateImageValidator : IValidator<List<CreateImageRequest>>
    {
    }
}
