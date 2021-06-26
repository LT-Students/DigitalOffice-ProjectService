using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class CreateTaskPropertyCommand : ICreateTaskPropertyCommand
    {
        private readonly IDbTaskPropertyMapper _mapper;
        private readonly IAccessValidator _accessValidator;
        private readonly IUserRepository _userRepository;
        private readonly ICreateTaskPropertyValidator _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITaskPropertyRepository _taskProperyRepository;

        public CreateTaskPropertyCommand(
            IDbTaskPropertyMapper mapper,
            IAccessValidator accessValidator,
            ITaskPropertyRepository taskProperyRepository,
            IUserRepository userRepository,
            ICreateTaskPropertyValidator validator,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _validator = validator;
            _userRepository = userRepository;
            _accessValidator = accessValidator;
            _taskProperyRepository = taskProperyRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public OperationResultResponse<IEnumerable<Guid>> Execute(CreateTaskPropertyRequest request)
        {
            _validator.ValidateAndThrowCustom(request);

            var userId = _httpContextAccessor.HttpContext.GetUserId();

            if (_accessValidator.IsAdmin() || _userRepository.AreUserProjectExist(userId, request.ProjectId))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            var dbTaskProperties = request.TaskProperties.Select(x => _mapper.Map(x));

            _taskProperyRepository.Create(dbTaskProperties);


            return new OperationResultResponse<IEnumerable<Guid>>
            {
                Body = dbTaskProperties.Select(x => x.Id),
                Status = OperationResultStatusType.FullSuccess
            };
        }
    }
}
