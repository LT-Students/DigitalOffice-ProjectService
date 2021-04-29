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

        private IEditProjectCommand _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _request = new JsonPatchDocument<EditProjectRequest>();
            _dbRequest = new JsonPatchDocument<DbProject>();
            _response = new OperationResultResponse<bool>();
        }

        [SetUp]
        public void SetUp()
        {
            _response.Body = true;
            _response.Status = OperationResultStatusType.FullSuccess;

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
        public void SuccessCommandWhenSuccessBrokerResponse()
        {
            Guid departmentId = Guid.NewGuid();

            var department = new Mock<IGetDepartmentResponse>();
            department.Setup(x => x.Id).Returns(departmentId);
            department.Setup(x => x.Name).Returns("Name");

            var operationResult = new Mock<IOperationResult<IGetDepartmentResponse>>();
            operationResult.Setup(x => x.Body).Returns(department.Object);
            operationResult.Setup(x => x.IsSuccess).Returns(true);

            var brokerResponse = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
            brokerResponse.Setup(x => x.Message).Returns(operationResult.Object);

            _mocker
                .Setup<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(null, departmentId), default, default))
                .Returns(Task.FromResult(brokerResponse.Object));

            var request = new JsonPatchDocument<EditProjectRequest>(
                new List<Operation<EditProjectRequest>>
                {
                    new Operation<EditProjectRequest>(
                        "replace",
                        $"/{nameof(EditProjectRequest.DepartmentId)}",
                        "",
                        departmentId
                    )
                }, new CamelCasePropertyNamesContractResolver());

            SerializerAssert.AreEqual(_response, _command.Execute(It.IsAny<Guid>(), request));
        }


        [Test]
        public void ValidationExceptionWhenInvalidRequest()
        {
            _mocker
                .Setup<IEditProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(It.IsAny<Guid>(), _request));
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
        }

        [Test]
        public void ArgumentNullExceptionWhenMapperThrowsIt()
        {
            _mocker
                .Setup<IPatchDbProjectMapper, JsonPatchDocument<DbProject>>(x => x.Map(_request))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _command.Execute(It.IsAny<Guid>(), _request));
        }

        [Test]
        public void NullReferenceExceptionWhenDbProjectNotFound()
        {
            _mocker
                .Setup<IProjectRepository, bool>(x => x.Edit(It.IsAny<Guid>(), _dbRequest))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => _command.Execute(It.IsAny<Guid>(), _request));
        }
    }
}