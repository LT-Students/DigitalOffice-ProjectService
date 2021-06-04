using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Responses.Company;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
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
        private DbProject _dbProject;
        private JsonPatchDocument<DbProject> _dbRequest;
        private OperationResultResponse<bool> _response;
        private Guid _departmentId = Guid.NewGuid();
        private Guid _departamentDirectorId = Guid.NewGuid();
        private Guid _notDepartmentDirectorUserId = Guid.NewGuid();
        IDictionary<object, object> _httpContextData;

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

            _dbProject = new DbProject { DepartmentId = _departmentId };
            _dbRequest = new JsonPatchDocument<DbProject>();
        }

        [SetUp]
        public void SetUp()
        {
            _response = new OperationResultResponse<bool>();
            _response.Body = true;
            _response.Status = OperationResultStatusType.FullSuccess;

            _getDepartmentResponse = new Mock<IGetDepartmentResponse>();
            _getDepartmentResponse.Setup(x => x.DepartmentId).Returns(_departmentId);
            _getDepartmentResponse.Setup(x => x.Name).Returns("Name");
            _getDepartmentResponse.Setup(x => x.DirectorUserId).Returns(_departamentDirectorId);

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
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            _mocker
                .Setup<IProjectRepository, DbProject>(x => x.GetProject(It.IsAny<GetProjectFilter>()))
                .Returns(_dbProject);

            _httpContextData = new Dictionary<object, object>();
            _httpContextData.Add("UserId", _departamentDirectorId);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_httpContextData);

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
                .Setup<IProjectRepository, bool>(x => x.Edit(_dbProject, _dbRequest))
                .Returns(true);

            _command = _mocker.CreateInstance<EditProjectCommand>();
        }

        [Test]
        public void SuccessCommandExecuteWhenRequesterDepartamentDirecorAndNotAdmin()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(false);

            SerializerAssert.AreEqual(_response, _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void SuccessCommandExecuteWhenRequesterAdminAndNotDepartmentDirector()
        {
            _getDepartmentResponse.Setup(x => x.DirectorUserId).Returns(_notDepartmentDirectorUserId);

            SerializerAssert.AreEqual(_response, _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void ValidationExceptionWhenInvalidRequest()
        {
            _mocker
                .Setup<IEditProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Never);
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, _departmentId), default, default),
                    Times.Never);
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Never);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<DbProject>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void ForbiddenExceptionWhenRequesterNotAdminAndNotDepartmetDirector()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(false);

            _getDepartmentResponse.Setup(x => x.DirectorUserId).Returns(_notDepartmentDirectorUserId);

            Assert.Throws<ForbiddenException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, _departmentId), default, default),
                    Times.Once);
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Never);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<DbProject>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void NotSuccessBrokerResponse()
        {
            _operationResult.Setup(x => x.IsSuccess).Returns(false);

            Assert.Throws<BadRequestException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, _departmentId), default, default), Times.Once);
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Never);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<DbProject>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void MapperArgumentNullExceptionWhenRequestIsNull()
        {
            _mocker
                .Setup<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request), Times.Once);
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<DbProject>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Never);
        }

        [Test]
        public void NullReferenceExceptionWhenDbProjectNotFound()
        {
            _mocker
                .Setup<IProjectRepository, bool>(x => x.Edit(It.IsAny<DbProject>(), _dbRequest))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => _command.Execute(It.IsAny<Guid>(), _request));
            _mocker.Verify<IProjectRepository, bool>(x => x.Edit(It.IsAny<DbProject>(), It.IsAny<JsonPatchDocument<DbProject>>()), Times.Once);
        }
    }
}