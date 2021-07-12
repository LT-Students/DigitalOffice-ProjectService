using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Validation.UnitTests
{
    internal class AddUsersToProjectValidatorTests
    {
        private IAddUsersToProjectValidator validator;

        private Mock<IUserRepository> _userRepository;
        private Mock<IProjectRepository> _projectRepository;

        private AddUsersToProjectRequest _request;
        private IEnumerable<DbProjectUser> _dbProjectUsers;

        private Guid _projectId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _userRepository = new Mock<IUserRepository>();
            _projectRepository = new Mock<IProjectRepository>();
            validator = new AddUsersToProjectValidator(_projectRepository.Object, _userRepository.Object);

            _dbProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projectId,
                    UserId = Guid.NewGuid(),
                    Role = (int) UserRoleType.ProjectAdmin,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _projectId,
                    UserId = Guid.NewGuid(),
                    Role = (int) UserRoleType.ProjectAdmin,
                    IsActive = true
                }
            };

            var projectUsers = new List<ProjectUserRequest>
            {
                new ProjectUserRequest
                {
                    Role = (int) UserRoleType.ProjectAdmin,
                    UserId = Guid.NewGuid()
                },
                new ProjectUserRequest
                {
                    Role = (int) UserRoleType.ProjectAdmin,
                    UserId = Guid.NewGuid()
                }
            };

            _request = new AddUsersToProjectRequest
            {
                ProjectId = _projectId,
                Users = projectUsers
            };
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdIsEmpty()
        {
            var emptyProjectId = Guid.Empty;

            validator.TestValidate(new AddUsersToProjectRequest()
                {
                    ProjectId = emptyProjectId,
                    Users = new List<ProjectUserRequest>() { }
                })
                .ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdDoesNotExist()
        {
            var showNotActiveUsers = false;

            IEnumerable<DbProjectUser> enumerable = new List<DbProjectUser>();
            _userRepository
                .Setup(x => x.GetProjectUsers(_request.ProjectId, showNotActiveUsers))
                .Returns(enumerable);

            validator.TestValidate(new AddUsersToProjectRequest()
                {
                    ProjectId = _request.ProjectId,
                    Users = new List<ProjectUserRequest>()
                    {
                        new ProjectUserRequest()
                        {
                            UserId = Guid.NewGuid(),
                            Role = UserRoleType.ProjectAdmin
                        }
                    }
                })
                .ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenUsersIsNull()
        {
            AddUsersToProjectRequest projectUsersRequest = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = null
            };

            validator
                .TestValidate(projectUsersRequest)
                .ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenListOfUsersIsEmpty()
        {
            AddUsersToProjectRequest projectUsersRequest = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = new List<ProjectUserRequest>()
            };

            validator
                .TestValidate(projectUsersRequest)
                .ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldHaveValidationErrorWhenUserAlreadyExist()
        {
            var showNotActiveUsers = false;

            _userRepository
                .Setup(x => x.GetProjectUsers(_request.ProjectId, showNotActiveUsers))
                .Returns(_dbProjectUsers);

            AddUsersToProjectRequest newRequest = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = new List<ProjectUserRequest>
                {
                    new ProjectUserRequest
                    {
                        Role = (int) UserRoleType.ProjectAdmin,
                        UserId = _dbProjectUsers.ElementAt(0).UserId
                    }
                }
            };

            validator.TestValidate(newRequest).ShouldHaveAnyValidationError();
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenRequestIsValid()
        {
            DbProject project = new DbProject {Id = _projectId};

            _projectRepository.Setup(p => p.IsExist(_projectId)).Returns(true);

            validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
        }
    }
}