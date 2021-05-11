using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindTasksCommand : IFindTasksCommand
    {
        private readonly ITaskInfoMapper _mapper;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccessValidator _accessValidator;
        private readonly ILogger<FindTasksCommand> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<IGetUsersDataRequest> _requestClient;

        private IGetUsersDataResponse GetUsersData(List<Guid> userId, List<string> errors)
        {
            string errorMessage = "Can not find user data. Please try again later.";

            try
            {
                var response = _requestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(
                    IGetUsersDataRequest.CreateObj(userId)).Result;

                if (response.Message.IsSuccess)
                {
                    return response.Message.Body;
                }

                errors.AddRange(response.Message.Errors);

                _logger.LogWarning("Can not find user data with this id {UserId}: " +
                    $"{Environment.NewLine}{string.Join('\n', response.Message.Errors)}", userId);
            }
            catch (Exception exc)
            {
                errors.Add(errorMessage);

                _logger.LogError(exc, errorMessage);
            }

            return null;
        }

        public FindTasksCommand(
            ITaskInfoMapper mapper,
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            ILogger<FindTasksCommand> logger,
            IAccessValidator accessValidator,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<IGetUsersDataRequest> requestClient)
        {
            _mapper = mapper;
            _logger = logger;
            _requestClient = requestClient;
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

            List<string> errors = new();

            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var projectUsers = _userRepository.Find(userId);

            if (!projectUsers.Any() && !_accessValidator.IsAdmin())
            {
                return new FindResponse<TaskInfo>();
            }

            var projectIds = projectUsers.Select(x => x.ProjectId);
            var dbTasks = _taskRepository.Find(filter, projectIds, skipCount, takeCount, out int totalCount);

            var users = dbTasks.Where(x => x.AssignedTo.HasValue).Select(x => x.AssignedTo.Value).ToList();
            users.AddRange(dbTasks.Select(x => x.AuthorId).ToList());

            var usersData = GetUsersData(users, errors);

            List<TaskInfo> tasks = new();
            foreach (var dbTask in dbTasks)
            {
                var assignedUser = usersData?.UsersData.FirstOrDefault(x => x.Id == dbTask.AssignedTo);
                var author = usersData?.UsersData.FirstOrDefault(x => x.Id == dbTask.AuthorId);

                tasks.Add(_mapper.Map(dbTask, assignedUser, author));
            }

            return new FindResponse<TaskInfo>
            {
                TotalCount = totalCount,
                Body = tasks,
                Errors = errors
            };
        }
    }
}
