using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class GetProjectsCommand : IGetProjectsCommand
    {
        private readonly IProjectRepository _repository;
        private readonly IProjectInfoMapper _mapper;

        public GetProjectsCommand(
            IProjectRepository repository,
            IProjectInfoMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<ProjectInfo> Execute(bool showNotActive)
        {
            return _repository.GetProjects(showNotActive).Select(p => _mapper.Map(p, null));
        }
    }
}