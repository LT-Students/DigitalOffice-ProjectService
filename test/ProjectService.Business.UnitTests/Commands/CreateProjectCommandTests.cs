//using FluentValidation;
//using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
//using LT.DigitalOffice.Kernel.Broker;
//using LT.DigitalOffice.Kernel.Constants;
//using LT.DigitalOffice.Kernel.Enums;
//using LT.DigitalOffice.Kernel.Exceptions.Models;
//using LT.DigitalOffice.Models.Broker.Requests.Company;
//using LT.DigitalOffice.Models.Broker.Requests.Message;
//using LT.DigitalOffice.Models.Broker.Responses.Company;
//using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
//using LT.DigitalOffice.ProjectService.Data.Interfaces;
//using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
//using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
//using LT.DigitalOffice.ProjectService.Models.Db;
//using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
//using LT.DigitalOffice.ProjectService.Models.Dto.Models;
//using LT.DigitalOffice.ProjectService.Models.Dto.Models.Employee;
//using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
//using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
//using LT.DigitalOffice.ProjectService.Validation.Interfaces;
//using LT.DigitalOffice.UnitTestKernel;
//using MassTransit;
//using Microsoft.AspNetCore.Http;
//using Moq;
//using Moq.AutoMock;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
//{
//    internal class CreateProjectCommandTests
//    {
//        private Guid _authorId;
//        private AutoMocker _mocker;
//        private DbProject _newDbProject;
//        private ProjectRequest _newRequest;
//        private ICreateProjectCommand _command;
//        private OperationResultResponse<ProjectInfo> _response;
//        private ProjectInfo _projectInfo;

//        private Mock<Response<IOperationResult<IGetDepartmentResponse>>> _operationResultGetDepartment;
//        private Mock<Response<IOperationResult<bool>>> _operationResultCreateWorkspace;

//        #region Setup

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _authorId = Guid.NewGuid();

//            _mocker = new AutoMocker();
//            _command = _mocker.CreateInstance<CreateProjectCommand>();

//            IDictionary<object, object> _items = new Dictionary<object, object>();
//            _items.Add("UserId", _authorId);

//            _mocker
//                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
//                .Returns(_items);

//            _newRequest = new ProjectRequest
//            {
//                Name = "Project for Lanit-Tercom",
//                ShortName = "Project",
//                Description = "New project for Lanit-Tercom",
//                ShortDescription = "Short description",
//                DepartmentId = Guid.NewGuid(),
//                Status = ProjectStatusType.Active,
//                Users = new List<ProjectUserRequest>
//                {
//                    new ProjectUserRequest
//                    {
//                        UserId = Guid.NewGuid(),
//                        Role = ProjectUserRoleType.Manager
//                    }
//                }
//            };

//            _newDbProject = new DbProject
//            {
//                Id = Guid.NewGuid(),
//                AuthorId = _authorId,
//                ShortName = _newRequest.ShortName,
//                DepartmentId = _newRequest.DepartmentId,
//                Name = _newRequest.Name,
//                Description = _newRequest.Description,
//                CreatedAt = DateTime.UtcNow,
//                ShortDescription = _newRequest.ShortDescription,
//                Status = (int)_newRequest.Status,
//                Users = new List<DbProjectUser>
//                {
//                    new DbProjectUser
//                    {
//                        Id = Guid.NewGuid(),
//                        Role = (int)_newRequest.Users.ElementAt(0).Role,
//                        IsActive = true
//                    }
//                }
//            };

//            _projectInfo = new ProjectInfo
//            {
//                Id = Guid.NewGuid(),
//                AuthorId = _authorId,
//                ShortName = _newRequest.ShortName,
//                Name = _newRequest.Name,
//                Description = _newRequest.Description,
//                CreatedAt = DateTime.UtcNow,
//                ShortDescription = _newRequest.ShortDescription,
//                Status = _newRequest.Status,
//                Department = new DepartmentInfo
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Some department"
//                }
//            };

//            _response = new OperationResultResponse<ProjectInfo>
//            {
//                Body = _projectInfo,
//                Status = OperationResultStatusType.FullSuccess,
//                Errors = new List<string>()
//            };

//            var department = new Mock<IGetDepartmentResponse>();
//            department.Setup(x => x.DepartmentId).Returns(_projectInfo.Department.Id);
//            department.Setup(x => x.Name).Returns(_projectInfo.Department.Name);

//            _operationResultGetDepartment = new Mock<Response<IOperationResult<IGetDepartmentResponse>>>();
//            _operationResultGetDepartment.Setup(x => x.Message.Body).Returns(department.Object);

//            _operationResultCreateWorkspace = new Mock<Response<IOperationResult<bool>>>();
//            _operationResultCreateWorkspace.Setup(x => x.Message.Body).Returns(true);
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            _mocker.GetMock<ICreateProjectValidator>().Reset();
//            _mocker.GetMock<IAccessValidator>().Reset();
//            _mocker.GetMock<IProjectInfoMapper>().Reset();
//            _mocker.GetMock<IDbProjectMapper>().Reset();
//            _mocker.GetMock<IProjectRepository>().Reset();
//            _mocker.GetMock<IRequestClient<IGetDepartmentRequest>>().Reset();
//            _mocker.GetMock<IRequestClient<ICreateWorkspaceRequest>>().Reset();

//            _operationResultGetDepartment.Setup(x => x.Message.IsSuccess).Returns(true);
//            _operationResultGetDepartment.Setup(x => x.Message.Errors).Returns(new List<string>());

//            _operationResultCreateWorkspace.Setup(x => x.Message.IsSuccess).Returns(true);
//            _operationResultCreateWorkspace.Setup(x => x.Message.Errors).Returns(new List<string>());

//            _mocker
//                .Setup<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
//                    x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//                        IGetDepartmentRequest.CreateObj(null, _newRequest.DepartmentId), default, TimeSpan.FromSeconds(2)))
//                .Returns(System.Threading.Tasks.Task.FromResult(_operationResultGetDepartment.Object));

//            _mocker
//                .Setup<IRequestClient<ICreateWorkspaceRequest>, Task<Response<IOperationResult<bool>>>>(
//                    x => x.GetResponse<IOperationResult<bool>>(
//                        It.IsAny<object>(), default, RequestTimeout.Default))
//                .Returns(System.Threading.Tasks.Task.FromResult(_operationResultCreateWorkspace.Object));
//        }

//        #endregion

//        [Test]
//        public void ShouldThrowExceptionWhenCreatingNewProjectWithIncorrectProjectData()
//        {
//            _mocker
//               .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
//               .Returns(true);

//            _mocker
//                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                .Returns(false);

//            Assert.Throws<ValidationException>(() => _command.Execute(_newRequest));
//            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
//            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//        }

//        [Test]
//        public void ShouldThrowExceptionWhenUserHasNotRights()
//        {
//            _mocker
//               .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
//               .Returns(false);

//            _mocker
//                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
//                .Returns(false);

//            Assert.Throws<ForbiddenException>(() => _command.Execute(_newRequest));

//            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
//            _mocker.Verify<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
//            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Never);
//            _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId), Times.Never);
//        }

//        [Test]
//        public void ShouldThrowExceptionWhenDepartmentNotFound()
//        {
//            _mocker
//                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
//                .Returns(true);

//            _mocker
//                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                .Returns(true);

//            _operationResultGetDepartment.Setup(x => x.Message.IsSuccess).Returns(false);
//            _operationResultGetDepartment.Setup(x => x.Message.Errors).Returns(new List<string>() { "Department was not found" });

//            Assert.Throws<BadRequestException>(() => _command.Execute(_newRequest));

//            _mocker.Verify<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
//            _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId), Times.Never);
//            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
//                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//                    IGetDepartmentRequest.CreateObj(null, _newRequest.DepartmentId), default, TimeSpan.FromSeconds(2)),
//                    Times.Once);
//            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//        }

//        [Test]
//        public void ShouldReturnFailResponseWhenRequestClientThrewError()
//        {
//            var newResponse = new OperationResultResponse<ProjectInfo>
//            {
//                Status = OperationResultStatusType.Failed,
//                Errors = new List<string>() { "Cannot add project. Please try again later." }
//            };

//            _mocker
//                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                .Returns(true);

//            _mocker
//                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
//                .Returns(true);

//            _mocker
//                .Setup<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId))
//                .Returns(_newDbProject);

//            _mocker
//                .Setup<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
//                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//                    IGetDepartmentRequest.CreateObj(null, _newRequest.DepartmentId), default, TimeSpan.FromSeconds(2)))
//                .Throws(new Exception());

//            SerializerAssert.AreEqual(newResponse, _command.Execute(_newRequest));

//            _mocker.Verify<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
//            _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId), Times.Never);
//            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
//                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//                    IGetDepartmentRequest.CreateObj(null, _newRequest.DepartmentId), default, TimeSpan.FromSeconds(2)),
//                    Times.Once);
//        }

//        [Test]
//        public void ShouldReturnResponseWhenCreatingNewProjectAndUserHasRights()
//        {
//            _mocker
//                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                .Returns(true);

//            _mocker
//                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
//                .Returns(true);

//            _mocker
//                .Setup<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId))
//                .Returns(_newDbProject);

//            _mocker
//                .Setup<IProjectInfoMapper, ProjectInfo>(x => x.Map(
//                    _newDbProject, _operationResultGetDepartment.Object.Message.Body.Name))
//                .Returns(_response.Body);

//            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
//        }

//        [Test]
//        public void ShouldReturnResponseWhenCreatingNewProjectAndUserIsAdmin()
//        {
//            _mocker
//                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                .Returns(true);

//            _mocker
//                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
//                .Returns(true);

//            _mocker
//                .Setup<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId))
//                .Returns(_newDbProject);

//            _mocker
//                .Setup<IProjectInfoMapper, ProjectInfo>(x => x.Map(
//                    _newDbProject, _operationResultGetDepartment.Object.Message.Body.Name))
//                .Returns(_response.Body);

//            SerializerAssert.AreEqual(_response, _command.Execute(_newRequest));
//        }

//        [Test]
//        public void ShouldReturnPartialsuccessResponseWhenWorkspaceWasNotCreated()
//        {
//            _mocker
//                .Setup<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
//                .Returns(true);

//            _mocker
//                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects))
//                .Returns(true);

//            _mocker
//                .Setup<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId))
//                .Returns(_newDbProject);

//            _mocker
//                .Setup<IProjectInfoMapper, ProjectInfo>(x => x.Map(
//                    _newDbProject, _operationResultGetDepartment.Object.Message.Body.Name))
//                .Returns(_response.Body);

//            _operationResultCreateWorkspace.Setup(x => x.Message.IsSuccess).Returns(false);
//            _operationResultCreateWorkspace.Setup(x => x.Message.Errors).Returns(new List<string>() { "some error" });

//            var response = new OperationResultResponse<ProjectInfo>
//            {
//                Body = _projectInfo,
//                Status = OperationResultStatusType.PartialSuccess,
//                Errors = new List<string>() { $"Failed to create a workspace for the project {_newRequest.Name}" }
//            };

//            SerializerAssert.AreEqual(response, _command.Execute(_newRequest));

//            _mocker.Verify<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveProjects), Times.Once);
//            _mocker.Verify<IDbProjectMapper, DbProject>(x => x.Map(_newRequest, _authorId), Times.Once);
//            _mocker.Verify<IRequestClient<IGetDepartmentRequest>, Task<Response<IOperationResult<IGetDepartmentResponse>>>>(
//                x => x.GetResponse<IOperationResult<IGetDepartmentResponse>>(
//                    IGetDepartmentRequest.CreateObj(null, _newRequest.DepartmentId), default, TimeSpan.FromSeconds(2)),
//                    Times.Once);
//            _mocker.Verify<IRequestClient<ICreateWorkspaceRequest>, Task<Response<IOperationResult<bool>>>>(
//                x => x.GetResponse<IOperationResult<bool>>(
//                    It.IsAny<object>(), default, RequestTimeout.Default),
//                    Times.Once);
//            _mocker.Verify<ICreateProjectValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid, Times.Once);
//        }
//    }
//}
