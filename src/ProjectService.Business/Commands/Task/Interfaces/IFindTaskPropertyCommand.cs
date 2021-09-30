using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Interfaces
{
  [AutoInject]
    public interface IFindTaskPropertyCommand
    {
        FindResultResponse<TaskPropertyInfo> Execute(FindTaskPropertiesFilter filter, int skipCount, int takeCount);
    }
}
