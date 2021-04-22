using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetProjectsCommand : IGetProjectsCommand
    {
        private readonly IProjectRepository _repository;
        private readonly IProjectResponseMapper _mapper;

        public GetProjectsCommand(
            IProjectRepository repository,
            IProjectResponseMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<ProjectInfo> Execute(bool showNotActive)
        {
            return _repository.GetProjects(showNotActive).Select(p => _mapper.Map(p));
        }
    }
}