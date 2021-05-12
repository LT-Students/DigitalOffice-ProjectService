using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    public class GetUserProjectsInfoConsumer : IConsumer<IGetUserProjectsInfoRequest>
    {
        private readonly IUserRepository _userRepository;

        private object GeUserProjects(Guid userId)
        {
            var dbProjectUsers = _userRepository.Find(userId);
            if (dbProjectUsers == null)
            {
                throw new EndpointNotFoundException();
            }

            var projectsResponse = new List<ProjectShortInfo>();

            foreach (var dbProjectUser in dbProjectUsers)
            {
                if (dbProjectUser.Project == null)
                {
                    continue;
                }

                projectsResponse.Add(new ProjectShortInfo
                {
                    Id = dbProjectUser.Project.Id,
                    Name = dbProjectUser.Project.Name,
                    Status = ((ProjectStatusType)dbProjectUser.Project.Status).ToString()
                });
            }
            return IGetUserProjectsInfoResponse.CreateObj(projectsResponse);
        }

        public GetUserProjectsInfoConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<IGetUserProjectsInfoRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GeUserProjects, context.Message.UserId);

            await context.RespondAsync<IOperationResult<IGetUserProjectsInfoResponse>>(response);
        }

    }
}
