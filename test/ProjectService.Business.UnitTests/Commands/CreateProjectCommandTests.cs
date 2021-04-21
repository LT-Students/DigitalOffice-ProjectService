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
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests.Commands
{
    internal class CreateProjectCommandTests
    {
        private ICreateNewProjectCommand _command;
        private Mock<IProjectRepository> _repositoryMock;
        private Mock<IProjectInfoMapper> _projectInfoMapper;
        private Mock<ILogger<CreateProjectCommand>> _loggerMock;
        private Mock<ICreateProjectValidator> _validatorMock;
        private Mock<IDbProjectMapper> _dbProjectMapperMock;
        private Mock<IAccessValidator> _accessValidator;
        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _operationResultBroker;
        private Mock<IRequestClient<IGetDepartmentRequest>> _requestClient;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;

        private DbProject _newDbProject;
        private ProjectRequest _newRequest;
        private OperationResultResponse<ProjectInfo> _response;
        private Guid _autorId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validatorMock = new Mock<ICreateProjectValidator>();
            _repositoryMock = new Mock<IProjectRepository>();
            _dbProjectMapperMock = new Mock<IDbProjectMapper>();
            _loggerMock = new Mock<ILogger<CreateProjectCommand>>();
            _accessValidator = new Mock<IAccessValidator>();
            _requestClient = new Mock<IRequestClient<IGetDepartmentRequest>>();
            _projectInfoMapper = new Mock<IProjectInfoMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _autorId = Guid.NewGuid();

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _autorId);

            _httpContextAccessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(_items);

            _command = new CreateProjectCommand(
                _repositoryMock.Object,
                _validatorMock.Object,
                _accessValidator.Object,
                _dbProjectMapperMock.Object,
                _projectInfoMapper.Object,
                _loggerMock.Object,
                _httpContextAccessorMock.Object,
                _requestClient.Object);

            _newRequest = new ProjectRequest
            {
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                DepartmentId = Guid.NewGuid(),
                Status = ProjectStatusType.Abandoned,
                Users = new List<ProjectUser>
                {
                    new ProjectUser
                    {
                        Id = Guid.NewGuid(),
                        Role = UserRoleType.Admin
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
                DepartmentId = _newRequest.DepartmentId,
                Name = _newRequest.Name,
                Description = _newRequest.Description,
                CreatedAt = DateTime.UtcNow,
                ShortDescription = _newRequest.ShortDescription,
                Status = _newRequest.Status,
            };

            _response = new OperationResultResponse<ProjectInfo>
            {
                Body = projectInfo,
                Status = OperationResultStatusType.FullSuccess,
                Errors = new List<string>()
            };

            _operationResultBroker = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
        }

        [SetUp]
        public void SetUp()
        {
            _validatorMock.Reset();
            _accessValidator.Reset();
            _dbProjectMapperMock.Reset();
            _dbProjectMapperMock.Reset();
            _repositoryMock.Reset();
            _requestClient.Reset();

            _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(true);
            _operationResultBroker.Setup(x => x.Message.Errors).Returns(new List<string>());

            _requestClient
                .Setup(x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default))
                .Returns(Task.FromResult(_operationResultBroker.Object));
        }

        [Test]
        public void ShouldThrowExceptionWhenCreatingNewProjectWithIncorrectProjectData()
        {
            _accessValidator
               .Setup(x => x.IsAdmin())
               .Returns(true);

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_newRequest));
            _accessValidator.Verify(x => x.IsAdmin(), Times.Once);
            _validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserHasNotRights()
        {
            _accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            _accessValidator
                .Setup(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(_newRequest));
            _accessValidator.Verify(x => x.IsAdmin(), Times.Once);
            _accessValidator.Verify(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
            _validatorMock.Verify(x => x.Validate(It.IsAny<IValidationContext>()), Times.Never);
            _dbProjectMapperMock.Verify(x => x.Map(_newRequest, _autorId), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenDepartmentNotFound()
        {
            _validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            _accessValidator
                .Setup(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(false);
            _operationResultBroker.Setup(x => x.Message.Errors).Returns(new List<string>() { "svsvs" });

            Assert.Throws<BadRequestException>(() => _command.Execute(_newRequest));
            _accessValidator.Verify(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
            _dbProjectMapperMock.Verify(x => x.Map(_newRequest, _autorId), Times.Never);
            _requestClient.Verify(x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default),
                    Times.Once);
            _validatorMock.Verify(x => x.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldReturnFailResponseWhenRequestClientThrewError()
        {
            var newResponse = new OperationResultResponse<ProjectInfo>
            {
                Status = OperationResultStatusType.Failed,
                Errors = new List<string>() { "Cannot add project. Please try again later." }
            };

            _validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            _accessValidator
                .Setup(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _dbProjectMapperMock
                .Setup(x => x.Map(_newRequest, _autorId))
                .Returns(_newDbProject);

            _requestClient
                .Setup(x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default))
                .Throws(new Exception());

            SerializerAssert.AreEqual(newResponse, _command.Execute(_newRequest));
            _accessValidator.Verify(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
            _dbProjectMapperMock.Verify(x => x.Map(_newRequest, _autorId), Times.Never);
            _validatorMock.Verify(x => x.Validate(It.IsAny<IValidationContext>()), Times.Once);
            _requestClient.Verify(x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
                    IGetDepartmentRequest.CreateObj(_newRequest.DepartmentId), default, default),
                    Times.Once);
        }

        [Test]
        public void ShouldReturnPartialResponseWhenCreatingNewProjectAndUserHasRights()
        {
            _validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            _accessValidator
                .Setup(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _dbProjectMapperMock
                .Setup(x => x.Map(_newRequest, _autorId))
                .Returns(_newDbProject);

            _projectInfoMapper
                .Setup(x => x.Map(_newDbProject))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
        }

        [Test]
        public void ShouldReturnResponseWhenCreatingNewProjectAndUserHasRights()
        {
            _validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            _accessValidator
                .Setup(x => x.HasRights(Rights.AddEditRemoveProjects))
                .Returns(true);

            _dbProjectMapperMock
                .Setup(x => x.Map(_newRequest, _autorId))
                .Returns(_newDbProject);

            _projectInfoMapper
                .Setup(x => x.Map(_newDbProject))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
        }

        [Test]
        public void ShouldReturnResponseWhenCreatingNewProjectAndUserIsAdmin()
        {
            _validatorMock
                 .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                 .Returns(true);

            _accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(true);

            _dbProjectMapperMock
                .Setup(x => x.Map(_newRequest, _autorId))
                .Returns(_newDbProject);

            _projectInfoMapper
                .Setup(x => x.Map(_newDbProject))
                .Returns(_response.Body);

            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
        }

       /* [Test]
        public void ShouldReturnIdWhenCreatingNewProjectAndUserHasRights()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.AreEqual(newProject.Id, command.Execute(newRequest));
            mapperMock.Verify();
            repositoryMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }

        [Test]
        public void ShouldReturnExceptionWhenUserIsNotAdminAndNotHasRights()
        {
            accessValidator
                .Setup(x => x.IsAdmin())
                .Returns(false);

            accessValidator
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => command.Execute(newRequest));
            //mapperMock.Verify(x => x.Map(It.IsAny<ProjectRequest>()), Times.Never);
            repositoryMock.Verify(x => x.CreateNewProject(It.IsAny<DbProject>()), Times.Never);
            validatorMock.Verify(x => x.Validate(It.IsAny<IValidationContext>()), Times.Never);
        }

        [Test]
        public void ShouldReturnIdWhenCreatingNewProject()
        {
            Assert.AreEqual(newProject.Id, command.Execute(newRequest));
            mapperMock.Verify();
            repositoryMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }*/
    }
}
