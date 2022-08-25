using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Business.Commands.Image;
using LT.DigitalOffice.ProjectService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Image;
using LT.DigitalOffice.ProjectService.Validation.Image.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands.Image
{
  internal class RemoveImageCommandTests
  {
    private IRemoveImageCommand _command;
    private AutoMocker _mocker;

    private Guid _projectId;
    private Guid _authorId;
    private List<Guid> _imagesIds;
    private RemoveImageRequest _request;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times removeImagesRequestValidatorTimes,
      Times projectUserRepositoryTimes,
      Times projectImageRepositoryTimes,
      Times publishTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IRemoveImagesRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, removeImagesRequestValidatorTimes);
      _mocker.Verify<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_authorId, _projectId, true), projectUserRepositoryTimes);
      _mocker.Verify<IProjectImageRepository, Task<bool>>(x => x.RemoveAsync(_request.ImagesIds), projectImageRepositoryTimes);
      _mocker.Verify<IPublish>(x => x.RemoveImagesAsync(_request.ImagesIds), publishTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<RemoveImageCommand>();

      _authorId = Guid.NewGuid();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _authorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _projectId = Guid.NewGuid();
      _imagesIds = new List<Guid>
      {
        Guid.NewGuid(),
        Guid.NewGuid()
      };

      _request = new RemoveImageRequest
      {
        ProjectId = _projectId,
        ImagesIds = _imagesIds
      };
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IProjectUserRepository>().Reset();
      _mocker.GetMock<IProjectImageRepository>().Reset();
      _mocker.GetMock<IPublish>().Reset();
      _mocker.GetMock<IRemoveImagesRequestValidator>().Reset();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(true);

      _mocker
        .Setup<IRemoveImagesRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
        .Returns(true);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Request is not correct." }
        });

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
        .Returns(new OperationResultResponse<bool>()
        {
          Errors = new() { "Not enough rights." }
        });
    }

    [Test]
    public async Task UserHasNotRights()
    {
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemoveProjects))
        .ReturnsAsync(false);

      _mocker
        .Setup<IProjectUserRepository, Task<bool>>(x => x.DoesExistAsync(_authorId, _projectId, true))
        .ReturnsAsync(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Not enough rights." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        removeImagesRequestValidatorTimes: Times.Never(),
        projectUserRepositoryTimes: Times.Once(),
        projectImageRepositoryTimes: Times.Never(),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task RequestIsNotCorrect()
    {
      _mocker
        .Setup<IRemoveImagesRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
        .Returns(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Errors = new List<string> { "Request is not correct." }
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Once(),
        removeImagesRequestValidatorTimes: Times.Once(),
        projectUserRepositoryTimes: Times.Once(),
        projectImageRepositoryTimes: Times.Never(),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task NotSuccessResult()
    {
      _mocker
        .Setup<IProjectImageRepository, Task<bool>>(x => x.RemoveAsync(_request.ImagesIds))
        .ReturnsAsync(false);

      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = false
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        removeImagesRequestValidatorTimes: Times.Once(),
        projectUserRepositoryTimes: Times.Once(),
        projectImageRepositoryTimes: Times.Once(),
        publishTimes: Times.Never());
    }

    [Test]
    public async Task SuccessResult()
    {
      _mocker
        .Setup<IProjectImageRepository, Task<bool>>(x => x.RemoveAsync(_request.ImagesIds))
        .ReturnsAsync(true);

      _mocker
        .Setup<IPublish>(x => x.RemoveImagesAsync(_request.ImagesIds));

      OperationResultResponse<bool> expectedResponse = new()
      {
        Body = true
      };

      SerializerAssert.AreEqual(expectedResponse, await _command.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        removeImagesRequestValidatorTimes: Times.Once(),
        projectUserRepositoryTimes: Times.Once(),
        projectImageRepositoryTimes: Times.Once(),
        publishTimes: Times.Once());
    }
  }
}
