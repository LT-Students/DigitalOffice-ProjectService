using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests.Commands
{
    internal class CreateProjectCommandTests
    {
        private Guid _autorId;
        private AutoMocker _mocker;
        private DbProject _newDbProject;
        private ProjectRequest _newRequest;
        private ICreateProjectCommand _command;
        private OperationResultResponse<ProjectInfo> _response;

        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _operationResultBroker;

        #region Setup

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _autorId = Guid.NewGuid();

            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<CreateProjectCommand>();

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _autorId);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _newRequest = new ProjectRequest
            {
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                DepartmentId = Guid.NewGuid(),
                Status = ProjectStatusType.Abandoned,
                Users = new List<ProjectUserRequest>
                {
                    new ProjectUserRequest
                    {
                        UserId = Guid.NewGuid(),
                        Role = UserRoleType.AdminProject
                    }
                }
            };

            _newDbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                AuthorId = _autorId,
                ShortName = _newRequest.ShortName,
                DepartmentId = _newRequest.DepartmentId,
                Name = _newRequest.Name,
                Description = _newRequest.Description,
                CreatedAt = DateTime.UtcNow,
                ShortDescription = _newRequest.ShortDescription,
                Status = (int)_newRequest.Status,
                Users = new List<DbProjectUser>
                {
                    new DbProjectUser
                    {
                        Id = Guid.NewGuid(),
                        Role = (int)_newRequest.Users.ElementAt(0).Role,
                        IsActive = true
                    }
                }
            };

            var projectInfo = new ProjectInfo
            {
                Id = Guid.NewGuid(),
                AuthorId = _autorId,
                ShortName = _newRequest.ShortName,
                Name = _newRequest.Name,
                Description = _newRequest.Description,
                CreatedAt = DateTime.UtcNow,
                ShortDescription = _newRequest.ShortDescription,
                Status = _newRequest.Status,
                Department = new DepartmentInfo
                {
                    Id = Guid.NewGuid(),
                    Name = "Some department"
                }
            };

            _response = new OperationResultResponse<ProjectInfo>
            {
                Body = projectInfo,
                Status = OperationResultStatusType.FullSuccess,
                Errors = new List<string>()
            };

            var department = new Mock<IGetDepartmentResponse>();
            department.Setup(x => x.Id).Returns(projectInfo.Department.Id);
            department.Setup(x => x.Name).Returns(projectInfo.Department.Name);

            _operationResultBroker = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
            _operationResultBroker.Setup(x => x.Message.Body).Returns(department.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<ICreateProjectValidator>().Reset();
            _mocker.GetMock<IAccessValidator>().Reset();
            _mocker.GetMock<IProjectInfoMapper>().Reset();
            _mocker.GetMock<IDbProjectMapper>().Reset();
            _mocker.GetMock<IProjectRepository>().Reset();
            _mocker.GetMock<IRequestClient<IGetDepartmentRequest>>().Reset();

            _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(true);
            _operationResultBroker.Setup(x => x.Message.Errors).Returns(new List<string>());

            _mocker
                .Setup<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default))
                .Returns(Task.FromResult(_operationResultBroker.Object));
        }

        #endregion

        [Test]
        public void ShouldThrowExceptionWhenCreatingNewProjectWithIncorrectProjectData()
        {
            _mocker
               .Setup<IAccessValidator, bool>(x => x.IsAdmin())
               .Returns(true);

            _mocker
                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_newRequest));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(), Times.Once);
            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserHasNotRights()
        {
            _mocker
               .Setup<IAccessValidator, bool>(x => x.IsAdmin())
               .Returns(false);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(_newRequest));

            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(), Times.Once);
            _mocker.Verify<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Never);
            _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _autorId), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenDepartmentNotFound()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _mocker
                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(false);
            _operationResultBroker.Setup(x => x.Message.Errors).Returns(new List<string>() { "Department was not found" });

            Assert.Throws<BadRequestException>(() => _command.Execute(_newRequest));

            _mocker.Verify<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
            _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _autorId), Times.Never);
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default),
                    Times.Once);
            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
        }

        [Test]
        public void ShouldReturnFailResponseWhenRequestClientThrewError()
        {
            var newResponse = new OperationResultResponse<ProjectInfo>
            {
                Status = OperationResultStatusType.Failed,
                Errors = new List<string>() { "Cannot add project. Please try again later." }
            };

            _mocker
                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _mocker
                .Setup<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _autorId))
                .Returns(_newDbProject);

            _mocker
                .Setup<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default))
                .Throws(new Exception());

            SerializerAssert.AreEqual(newResponse, _command.Execute(_newRequest));

            _mocker.Verify<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
            _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _autorId), Times.Never);
            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default),
                    Times.Once);
        }

        [Test]
        public void ShouldReturnResponseWhenCreatingNewProjectAndUserHasRights()
        {
            _mocker
                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _mocker
                .Setup<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _autorId))
                .Returns(_newDbProject);

            _mocker
                .Setup<IProjectInfoMapper, ProjectInfo>(x => x.Map(_newDbProject, _operationResultBroker.Object.Message.Body))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
        }

        [Test]
        public void ShouldReturnResponseWhenCreatingNewProjectAndUserIsAdmin()
        {
            _mocker
                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _mocker
                .Setup<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _autorId))
                .Returns(_newDbProject);

            _mocker
                .Setup<IProjectInfoMapper, ProjectInfo>(x => x.Map(_newDbProject, _operationResultBroker.Object.Message.Body))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
        }
    }
}
