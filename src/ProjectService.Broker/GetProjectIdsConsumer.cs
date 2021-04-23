using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Models.Broker.Requests;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker
{
    class GetProjectsConsumer : IConsumer<IGetProjectRequest>
    {
        public async Task Consume(ConsumeContext<IGetProjectRequest> context)
        {
            await context.RespondAsync<IOperationResult<bool>>();
        }
    }
}
