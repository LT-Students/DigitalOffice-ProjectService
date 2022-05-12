using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;

namespace LT.DigitalOffice.ProjectService.Business.Commands.ProjectUsers
{
  internal class EditProjectUsersCommand : IEditProjectUsersCommand
  {
    public Task<OperationResultResponse<bool>> ExecuteAsync(EditProjectUsersRequest request)
    {
      throw new NotImplementedException();
    }
  }
}
