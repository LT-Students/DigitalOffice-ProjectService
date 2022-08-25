using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Image;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Image
{
  public class CreateImageCommand : ICreateImageCommand
  {
    private readonly IProjectImageRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbImageMapper _dbProjectImageMapper;
    private readonly ICreateImagesRequestValidator _validator;
    private readonly IProjectUserRepository _userRepository;
    private readonly IResponseCreator _responseCreator;
    private readonly IImageService _imageService;

    public CreateImageCommand(
      IProjectImageRepository repository,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDbImageMapper dbProjectImageMapper,
      ICreateImagesRequestValidator validator,
      IProjectUserRepository userRepository,
      IResponseCreator responseCreator,
      IImageService imageService)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _dbProjectImageMapper = dbProjectImageMapper;
      _validator = validator;
      _userRepository = userRepository;
      _responseCreator = responseCreator;
      _imageService = imageService;
    }

    public async Task<OperationResultResponse<List<Guid>>> ExecuteAsync(CreateImagesRequest request)
    {
      if (!await _userRepository.DoesExistAsync(userId: _httpContextAccessor.HttpContext.GetUserId(), projectId: request.ProjectId, isManager: true)
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects))
      {
        return _responseCreator.CreateFailureResponse<List<Guid>>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<List<Guid>>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(x => x.ErrorMessage).ToList());
      }

      OperationResultResponse<List<Guid>> response = new();

      List<Guid> imagesIds = await _imageService.CreateImagesAsync(request.Images, response.Errors);

      if (response.Errors.Any())
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return response;
      }

      response.Body = await _repository.CreateAsync(imagesIds.Select(imageId =>
        _dbProjectImageMapper.Map(request, imageId))
        .ToList());

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
