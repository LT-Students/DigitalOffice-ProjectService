using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindTasksCommand : IFindTasksCommand
    {
        private readonly ITaskRepository _repository;
        private readonly ITaskInfoMapper _mapper;
        public FindTasksCommand(ITaskInfoMapper mapper, ITaskRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public FindResponse<TaskInfo> Execute(FindTasksFilter filter, int skipCount, int takeCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var dbTasks = _repository.Find(filter, skipCount, takeCount, out int totalCount);

            var tasks = dbTasks.Select(x => _mapper.Map(x));

            return new FindResponse<TaskInfo>
            {
                TotalCount = totalCount,
                Body = tasks
            };
        }
    }
}
