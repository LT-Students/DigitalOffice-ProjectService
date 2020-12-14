using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    /// <summary>
    /// Consumer for getting information about the user.
    /// </summary>
    public class GetProjectInfoConsumer : IConsumer<IGetProjectRequest>
    {
        private readonly IProjectRepository repository;

        public GetProjectInfoConsumer(
            [FromServices] IProjectRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<IGetProjectRequest> context)
        {
            var response = OperationResultWrapper.CreateResponse(GetProjectInfo, context.Message);

            await context.RespondAsync<IOperationResult<IGetProjectResponse>>(response);
        }

        private object GetProjectInfo(IGetProjectRequest request)
        {
            var dbProject = repository.GetProject(request.ProjectId);

            if (dbProject == null)
            {
                throw new NotFoundException();
            }

            return IGetProjectResponse.CreateObj(dbProject.Id, dbProject.IsActive);
        }
    }
}
