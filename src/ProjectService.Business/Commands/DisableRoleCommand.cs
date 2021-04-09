using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using System;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class DisableRoleCommand : IDisableRoleCommand
    {
        private readonly IRoleRepository repository;
        private readonly IAccessValidator accessValidator;

        public DisableRoleCommand(
            IRoleRepository repository,
            IAccessValidator accessValidator)
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

            return repository.DisableRole(roleId);
        }
    }
}
