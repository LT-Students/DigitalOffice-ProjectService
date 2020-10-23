using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;

namespace LT.DigitalOffice.ProjectService.Mappers
{
    public class WorkerMapper : IMapper<ProjectWorker, DbProjectWorkerUser>
    {
        public DbProjectWorkerUser Map(ProjectWorker projectWorker)
        {
            return new DbProjectWorkerUser
            {
                WorkerUserId = projectWorker.WorkerId,

            };
        }
    }
}
