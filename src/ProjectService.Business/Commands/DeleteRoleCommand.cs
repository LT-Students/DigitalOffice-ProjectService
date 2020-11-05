using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class DeleteRoleCommand : IDeleteRoleCommand
    {
        private readonly IRoleRepository repository;
        private readonly IAccessValidator accessValidator;

        public DeleteRoleCommand(
            [FromServices] IRoleRepository repository,
            [FromServices] IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.accessValidator = accessValidator;
        }

        public bool Execute(Guid roleId)
        {
            const int accessRightId = 2;

            if (!(accessValidator.IsAdmin() || accessValidator.HasRights(accessRightId)))
            {
                throw new ForbiddenException("Not enough rights");
            }

            return repository.DeleteRole(roleId);
        }
    }
}
