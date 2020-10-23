using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateRoleCommand : ICreateRoleCommand
    {
        private readonly IRoleRepository repository;
        private readonly IValidator<CreateRoleRequest> validator;
        private readonly IMapper<CreateRoleRequest, DbRole> mapper;

        public CreateRoleCommand(
            [FromServices] IRoleRepository repository,
            [FromServices] IValidator<CreateRoleRequest> validator,
            [FromServices] IMapper<CreateRoleRequest, DbRole> mapper)
        {
            this.repository = repository;
            this.validator = validator;
            this.mapper = mapper;
        }

        public Guid Execute(CreateRoleRequest request)
        {
            validator.ValidateAndThrowCustom(request);

            return repository.CreateRole(mapper.Map(request));
        }
    }
}
