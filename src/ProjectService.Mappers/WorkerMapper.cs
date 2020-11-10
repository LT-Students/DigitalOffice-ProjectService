using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;

namespace LT.DigitalOffice.ProjectService.Mappers
{
    public class WorkerMapper : IMapper<ProjectUser, DbProjectUser>
    {
        public DbProjectUser Map(ProjectUser projectWorker)
        {
            return new DbProjectUser
            {
                UserId = projectWorker.User.Id,
            };
        }
    }
}
