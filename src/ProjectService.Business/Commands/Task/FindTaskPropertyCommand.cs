using LT.DigitalOffice.ProjectService.Business.Commands.Task.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindTaskPropertyCommand : IFindTaskPropertyCommand
    {
        private readonly ITaskPropertyInfoMapper _mapper;
        private readonly ITaskPropertyRepository _repository;

        public FindTaskPropertyCommand(
            ITaskPropertyRepository repository,
            ITaskPropertyInfoMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public FindResponse<TaskPropertyInfo> Execute(Guid? projectId, string name, int skipCount, int tackeCount)
        {
            var dbTaskProperties = _repository.Find(projectId, name, skipCount, tackeCount, out int totalCount);

            var taskProperties = dbTaskProperties.Select(tp => _mapper.Map(tp));

            return new FindResponse<TaskPropertyInfo>
            {
                Body = dbTaskProperties.Select(tp => _mapper.Map(tp)),
                TotalCount = totalCount
            };
        }
    }
}
