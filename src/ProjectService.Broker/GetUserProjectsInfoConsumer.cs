using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Data.Interfaces;

using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.Broker.Requests;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetUserProjectsInfoConsumer : IConsumer<IGetUserProjectsInfoRequest>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private object GeUserProjects(Guid userId)
        {
            var dbUserProjects = _userRepository.Find(userId);
            if (dbUserProjects == null)
            {
                throw new EndpointNotFoundException();
            }

            var projectsResponse = new List<ProjectShortInfo>();

            foreach (var dbUserProject in dbUserProjects)
            {
                var dbProject = _projectRepository.GetProject(dbUserProject.ProjectId);

                projectsResponse.Add(new ProjectShortInfo {
                    Id = dbProject.Id,
                    Name = dbProject.Name,
                    Status = ((ProjectStatusType)dbProject.Status).ToString()
                });
            }
            return IGetUserProjectsInfoResponse.CreateObj(projectsResponse);
        }

        public GetUserProjectsInfoConsumer(
            IUserRepository userRepository,
            IProjectRepository projectRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        public async Task Consume(ConsumeContext<IGetUserProjectsInfoRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GeUserProjects, context.Message.UserId);

            await context.RespondAsync<IOperationResult<IGetUserProjectsInfoResponse>>(response);
        }

    }
}
