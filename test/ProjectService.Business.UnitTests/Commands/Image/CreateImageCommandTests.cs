using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Image;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands.Image
{
  internal class CreateImageCommandTests
  {
    private Guid _authorId;
    private Guid _projectId;
    private AutoMocker _mocker;
    private CreateImagesRequest _request;
    private ICreateImageCommand _command;
    private List<Guid> _imagesIds;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times сreateImagesRequestValidatorTimes,
      Times imageServiceTimes,
      Times projectUserRepositoryTimes,
      Times dbImageMapperTimes,
      Times projectImageRepositoryTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<ICreateImagesRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<CreateImagesRequest>(), default).Result.IsValid, сreateImagesRequestValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<List<Guid>>>(
        x => x.CreateFailureResponse<List<Guid>>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IImageService, Task<List<Guid>>>(x => x.CreateImagesAsync(It.IsAny<List<ImageContent>>(), It.IsAny<List<string>>()), imageServiceTimes);
      _mocker.Verify<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_authorId, _request.ProjectId, true), projectUserRepositoryTimes);
      _mocker.Verify<IDbImageMapper, DbProjectImage>(x => x.Map(_request, It.IsAny<Guid>()), dbImageMapperTimes);
      _mocker.Verify<IProjectImageRepository, Task<List<Guid>>>(x => x.CreateAsync(It.IsAny<List<DbProjectImage>>()), projectImageRepositoryTimes);

      _mocker.Resolvers.Clear();
    }

    #region Setup

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _authorId = Guid.NewGuid();
      _projectId = Guid.NewGuid();

      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<CreateImageCommand>();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _authorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _request = new CreateImagesRequest
      {
        ProjectId = _projectId
      };

      _imagesIds = new List<Guid>
      {
        Guid.NewGuid(),
        Guid.NewGuid()
      };

      _mocker
        .Setup<IHttpContextAccessor, int>(a => a.HttpContext.Response.StatusCode)
        .Returns(200);
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<ICreateImagesRequestValidator>().Reset();
      _mocker.GetMock<IImageService>().Reset();
      _mocker.GetMock<IProjectUserRepository>().Reset();
      _mocker.GetMock<IProjectImageRepository>().Reset();
      _mocker.GetMock<IDbImageMapper>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(true);

      _mocker
        .Setup<ICreateImagesRequestValidator, bool>(x => x.ValidateAsync(It.IsAny<CreateImagesRequest>(), default).Result.IsValid)
        .Returns(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<List<Guid>>>(x => x.CreateFailureResponse<List<Guid>>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<List<Guid>>()
        {
          Errors = new() { "Request is not correct." }
        });

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<List<Guid>>>(x => x.CreateFailureResponse<List<Guid>>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<List<Guid>>()
        {
          Errors = new() { "Not enough rights." }
        });
    }

    #endregion

    [Test]
    public async Task UserHasNotRights()
    {
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(false);

      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_authorId, _request.ProjectId, true))
        .ReturnsAsync(false);

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string> { "Not enough rights." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        сreateImagesRequestValidatorTimes: Times.Never(),
        imageServiceTimes: Times.Never(),
        projectUserRepositoryTimes: Times.Once(),
        dbImageMapperTimes: Times.Never(),
        projectImageRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task RequestIsNotCorrect()
    {
      _mocker
        .Setup<ICreateImagesRequestValidator, bool>(x => x.ValidateAsync(_request, default).Result.IsValid)
        .Returns(false);

      OperationResultResponse<Guid?> expectedResponse = new()
      {
        Errors = new List<string> { "Request is not correct." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        сreateImagesRequestValidatorTimes: Times.Once(),
        imageServiceTimes: Times.Never(),
        projectUserRepositoryTimes: Times.Once(),
        dbImageMapperTimes: Times.Never(),
        projectImageRepositoryTimes: Times.Never());
    }

    [Test]
    public async Task ResultTestWithCreationWorkTime()
    {
      _mocker
        .Setup<IImageService, Task<List<Guid>>>(x => x.CreateImagesAsync(It.IsAny<List<ImageContent>>(), It.IsAny<List<string>>()))
        .ReturnsAsync(_imagesIds);

      _mocker
        .Setup<IProjectImageRepository, Task<List<Guid>>>(x => x.CreateAsync(It.IsAny<List<DbProjectImage>>()))
        .ReturnsAsync(_imagesIds);

      _mocker
        .Setup<IDbImageMapper, DbProjectImage>(x => x.Map(_request, It.IsAny<Guid>()))
        .Returns(new DbProjectImage());

      OperationResultResponse<List<Guid>> expectedResponse = new()
      {
        Body = _imagesIds,
        Errors = new List<string> { }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        сreateImagesRequestValidatorTimes: Times.Once(),
        imageServiceTimes: Times.Once(),
        projectUserRepositoryTimes: Times.Once(),
        dbImageMapperTimes: Times.Exactly(2),
        projectImageRepositoryTimes: Times.Once());
    }
  }
}
