using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindTasksCommand : IFindTasksCommand
    {
        private readonly ITaskInfoMapper _mapper;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccessValidator _accessValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FindTasksCommand(
            ITaskInfoMapper mapper,
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IAccessValidator accessValidator)
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _accessValidator = accessValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        public FindResponse<TaskInfo> Execute(FindTasksFilter filter, int skipCount, int takeCount)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var projectUsers = _userRepository.Find(userId);

            bool isAdmin = _accessValidator.IsAdmin();
            if (!projectUsers.Any() && !isAdmin)
            {
                throw new ForbiddenException("Not enough rights.");
            }

            var projectIds = projectUsers.Select(x => x.ProjectId).Distinct();
            var dbTasks = _taskRepository.Find(filter, projectIds, skipCount, takeCount, out int totalCount);

            var tasks = dbTasks.Select(x => _mapper.Map(x));

            return new FindResponse<TaskInfo>
            {
                TotalCount = totalCount,
                Body = tasks
            };
        }
    }
}
