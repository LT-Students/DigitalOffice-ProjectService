using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for adding a new task.
    /// </summary>
    [AutoInject]
    public interface ICreateNewTaskCommand
    {
        /// <summary>
        /// Adds a new task. Returns id of the added task.
        /// </summary>
        /// <param name="request">Project data.</param>
        /// <returns>Id of the added task.</returns>
        OperationResultResponse<Guid> Execute(CreateTaskRequest request);
    }
}
