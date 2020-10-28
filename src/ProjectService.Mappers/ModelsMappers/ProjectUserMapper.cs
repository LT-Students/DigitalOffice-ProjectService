using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class ProjectUserMapper : IProjectUserMapper
    {
        private readonly ILogger<ProjectUserMapper> _logger;
        private readonly IRoleMapper _roleMapper;
        private readonly IRequestClient<IGetUserRequest> _requestClient;

        public ProjectUserMapper(
            ILogger<ProjectUserMapper> logger,
            IRoleMapper roleMapper,
            IRequestClient<IGetUserRequest> requestClient)
        {
            _logger = logger;
            _roleMapper = roleMapper;
            _requestClient = requestClient;
        }

        public async Task<ProjectUser> Map(DbProjectUser dbProjectUser)
        {
            if (dbProjectUser == null)
            {
                throw new ArgumentNullException(nameof(dbProjectUser));
            }

            var user = new User
            {
                Id = dbProjectUser.UserId,
            };

            try
            {
                var userInfoResponse = await _requestClient.GetResponse<IOperationResult<IGetUserResponse>>(
                    IGetUserRequest.CreateObj(dbProjectUser.UserId));

                if (userInfoResponse.Message.IsSuccess)
                {
                    user.FirstName = userInfoResponse.Message.Body.FirstName;
                    user.MiddleName = userInfoResponse.Message.Body.MiddleName;
                    user.LastName = userInfoResponse.Message.Body.LastName;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception on get user information.");
            }

            return new ProjectUser
            {
                User = user,
                Role = _roleMapper.Map(dbProjectUser.Role)
            };
        }
    }
}
