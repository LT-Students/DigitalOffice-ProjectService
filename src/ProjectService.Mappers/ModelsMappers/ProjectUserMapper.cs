using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.User;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class ProjectUserMapper : IProjectUserMapper
    {
        private readonly ILogger<ProjectUserMapper> _logger;
        private readonly IRequestClient<IGetUserDataRequest> _requestClient;

        public ProjectUserMapper(
            ILogger<ProjectUserMapper> logger,
            IRequestClient<IGetUserDataRequest> requestClient)
        {
            _logger = logger;
            _requestClient = requestClient;
        }

        public async Task<ProjectUserInfo> Map(DbProjectUser dbProjectUser)
        {
            if (dbProjectUser == null)
            {
                throw new ArgumentNullException(nameof(dbProjectUser));
            }

            var user = new UserInfo
            {
                Id = dbProjectUser.UserId,
            };

            try
            {
                var userDataResponse = await _requestClient.GetResponse<IOperationResult<IGetUserDataResponse>>(
                    IGetUserDataRequest.CreateObj(dbProjectUser.UserId));

                if (userDataResponse.Message.IsSuccess)
                {
                    user.FirstName = userDataResponse.Message.Body.FirstName;
                    user.MiddleName = userDataResponse.Message.Body.MiddleName;
                    user.LastName = userDataResponse.Message.Body.LastName;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception on get user information.");
            }

            return new ProjectUserInfo
            {
                ProjectId = dbProjectUser.ProjectId,
                User = user,
                Role = (UserRoleType)dbProjectUser.Role
            };
        }
    }
}
