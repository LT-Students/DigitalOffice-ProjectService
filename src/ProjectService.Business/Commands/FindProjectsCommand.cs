using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindProjectsCommand : IFindProjectsCommand
    {
        private readonly IProjectRepository _repository;
        private readonly IProjectResponseMapper _mapper;

        public FindProjectsCommand(
            IProjectRepository repository,
            IProjectResponseMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public ProjectsResponse Execute(FindProjectsFilter filter, int skipCount, int takeCount)
        {
            throw new System.NotImplementedException();
        }
    }
}
