using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.ProjectService.Validation.Project.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project
{
  public class CreateProjectCommand : ICreateProjectCommand
  {
    private readonly IProjectRepository _repository;
    private readonly IDbProjectMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateProjectRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IImageService _imageService;
    private readonly IPublish _publish;
    private readonly IGlobalCacheRepository _globalCache;

    public CreateProjectCommand(
      IProjectRepository repository,
      ICreateProjectRequestValidator validator,
      IAccessValidator accessValidator,
      IDbProjectMapper mapper,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IImageService imageService,
      IPublish publish,
      IGlobalCacheRepository globalCache)
    {
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _imageService = imageService;
      _publish = publish;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateProjectRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      List<Guid> imagesIds = await _imageService.CreateImagesAsync(request.ProjectImages, response.Errors);
      DbProject dbProject = _mapper.Map(request, imagesIds);

      await _repository.CreateAsync(dbProject);

      if (request.Users.Any() && request.Status == (int)ProjectStatusType.Active)
      {
        await _publish.CreateWorkTimeAsync(
          dbProject.Id,
          usersIds: request.Users.Select(u => u.UserId).ToList());
      }

      if (request.DepartmentId.HasValue)
      {
        await _globalCache.RemoveAsync(request.DepartmentId.Value);
      }
      request.Users?.ForEach(async user => await _globalCache.RemoveAsync(user.UserId));

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      response.Body = dbProject.Id;
      return response;
    }
  }
}
