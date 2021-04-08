using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces
{
    [AutoInject]
    public interface IProjectExpandedResponseMapper
    {
        Task<ProjectExpandedResponse> Map(DbProject dbProject, IEnumerable<DbProjectUser> dbProjectUsers);
    }
}
