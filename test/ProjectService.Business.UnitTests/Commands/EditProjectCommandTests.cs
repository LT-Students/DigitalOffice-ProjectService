using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Commands
{
    internal class EditProjectCommandTests
    {
        private AutoMocker _mocker;
        private JsonPatchDocument<EditProjectRequest> _request;
        private JsonPatchDocument<DbProject> _dbRequest;
        private OperationResultResponse<bool> _response;
        private Guid _departmentId = Guid.NewGuid();

        private Mock<IGetDepartmentResponse> _getDepartmentResponse;
        private Mock<IOperationResult<IGetDepartmentResponse>> _operationResult;
        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _brokerResponse;

        private IEditProjectCommand _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _request = new JsonPatchDocument<EditProjectRequest>(
                new List<Operation<EditProjectRequest>>
                {
                    new Operation<EditProjectRequest>(
                        "replace",
                        $"/{nameof(EditProjectRequest.DepartmentId)}",
                        "",
                        _departmentId)
                }, new CamelCasePropertyNamesContractResolver());
            _dbRequest = new JsonPatchDocument<DbProject>();
        }

        [SetUp]
        public void SetUp()
        {
            _response = new OperationResultResponse<bool>();
            _response.Body = true;
            _response.Status = OperationResultStatusType.FullSuccess;

            _getDepartmentResponse = new Mock<IGetDepartmentResponse>();
            _getDepartmentResponse.Setup(x => x.Id).Returns(_departmentId);
            _getDepartmentResponse.Setup(x => x.Name).Returns("Name");

            _operationResult = new Mock<IOperationResult<IGetDepartmentResponse>>();
            _operationResult.Setup(x => x.Body).Returns(_getDepartmentResponse.Object);
            _operationResult.Setup(x => x.IsSuccess).Returns(true);

            _brokerResponse = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
            _brokerResponse.Setup(x => x.Message).Returns(_operationResult.Object);

            _mocker = new AutoMocker();
            _mocker
                .Setup<IEditProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin())
                .Returns(true);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            _mocker
                .Setup<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, _departmentId), default, default))
                .Returns(Task.FromResult(_brokerResponse.Object));

            _mocker
                .Setup<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x =>
                    x.Map(It.IsAny<JsonPatchDocument<EditProjectRequest>>()))
                .Returns(_dbRequest);

            _mocker
                .Setup<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), _dbRequest))
                .Returns(true);

            _command = _mocker.CreateInstance<EditProjectCommand>();
        }

        [Test]
        public void SuccessCommandExecuteWhenAdminandHasRights()
        {
            SerializerAssert.AreEqual(_response, _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void SuccessCommandExecuteWhenNotAdmin()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin())
                .Returns(false);

            SerializerAssert.AreEqual(_response, _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void SuccessCommandExecuteWhenNotRights()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            SerializerAssert.AreEqual(_response, _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void ValidationExceptionWhenInvalidRequest()
        {
            _mocker
                .Setup<IEditProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(), Times.Never);
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, _departmentId), default, default),
                    Times.Never);
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Never);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void ForbiddenExceptionWhenUserIsNotAdminAndHasNoRights()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin())
                .Returns(false);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(), Times.Once);
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, _departmentId), default, default),
                    Times.Never);
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Never);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void NotSuccessBrokerResponse()
        {
            _operationResult.Setup(x => x.IsSuccess).Returns(false);
            _response.Body = false;
            _response.Status = OperationResultStatusType.Failed;
            _response.Errors.Add("Cannot edit project. Please try again later.");

            SerializerAssert.AreEqual(_response, _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, _departmentId), default, default), Times.Once);
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Never);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void ArgumentNullExceptionWhenMapperThrowsIt()
        {
            _mocker
                .Setup<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Once);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void NullReferenceExceptionWhenDbProjectNotFound()
        {
            _mocker
                .Setup<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), _dbRequest))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Once);
        }
    }
}