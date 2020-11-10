using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateRoleCommand : ICreateRoleCommand
    {
        private readonly IRoleRepository repository;
        private readonly IValidator<CreateRoleRequest> validator;
        private readonly ICreateRoleRequestMapper mapper;

        public CreateRoleCommand(
            [FromServices] IRoleRepository repository,
            [FromServices] IValidator<CreateRoleRequest> validator,
            [FromServices] ICreateRoleRequestMapper mapper)
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
