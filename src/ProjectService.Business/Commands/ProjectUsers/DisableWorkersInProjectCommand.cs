using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
    public class DisableWorkersInProjectCommand : IDisableWorkersInProjectCommand
    {
        private readonly IProjectRepository repository;
        private readonly IAccessValidator accessValidator;

        public DisableWorkersInProjectCommand(
            IProjectRepository repository,
            IAccessValidator accessValidator)
        {
            this.repository = repository;
            this.accessValidator = accessValidator;
        }

        public void Execute(Guid projectId, IEnumerable<Guid> userIds)
        {
            if (userIds == null || userIds.Count() == 0)
            {
                throw new BadRequestException("Users not specified.");
            }

            if (!(accessValidator.IsAdmin() || accessValidator.HasRights(Rights.AddEditRemoveProjects)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            repository.DisableWorkersInProject(projectId, userIds);
        }
    }
}
