using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
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

        private Mock<IProjectRepository> _repository;

        private AddUsersToProjectRequest _request;
        private IEnumerable<DbProjectUser> _dbProjectUsers;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IProjectRepository>();
            validator = new AddUsersToProjectValidator(_repository.Object);

            var projectId = Guid.NewGuid();

            _dbProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    RoleId = Guid.NewGuid(),
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    RoleId = Guid.NewGuid(),
                    IsActive = true
                }
            };

            var users = new List<UserRequest>
            {
                new UserRequest
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                },
                new UserRequest
                {
                    Id = Guid.NewGuid(),
                    IsActive = true
                }
            };

            var projectUsers = new List<ProjectUserRequest>
            {
                new ProjectUserRequest
                {
                    RoleId = Guid.NewGuid(),
                    User = users.ElementAt(0)
                },
                new ProjectUserRequest
                {
                    RoleId = Guid.NewGuid(),
                    User = users.ElementAt(1)
                }
            };

            _request = new AddUsersToProjectRequest
            {
                ProjectId = projectId,
                Users = projectUsers
            };
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdIsEmpty()
        {
            var emptyProjectId = Guid.Empty;

            validator.ShouldHaveValidationErrorFor(x => x.ProjectId, emptyProjectId);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenProjectIdDoesNotExist()
        {
            var showNotActiveUsers = false;

            _repository
                .Setup(x => x.GetProjectUsers(_request.ProjectId, showNotActiveUsers))
                .Returns<IEnumerable<DbProjectUser>>(null)
                .Verifiable();

            validator.ShouldHaveValidationErrorFor(x => x.ProjectId, _request.ProjectId);
            _repository.Verify();
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenUsersIsNull()
        {
            AddUsersToProjectRequest projectUser = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = null
            };

            validator.ShouldHaveValidationErrorFor(x => x.Users, projectUser);
        }

        [Test]
        public void ShouldHaveValidationErrorForWhenListOfUsersIsEmpty()
        {
            AddUsersToProjectRequest projectUser = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = new List<ProjectUserRequest>()
            };

            validator.ShouldHaveValidationErrorFor(x => x.Users, projectUser);
        }

        [Test]
        public void ShouldHaveValidationErrorWhenUserAlreadyExist()
        {
            var showNotActiveUsers = false;

            _repository
                .Setup(x => x.GetProjectUsers(_request.ProjectId, showNotActiveUsers))
                .Returns(_dbProjectUsers);

            AddUsersToProjectRequest newRequest = new AddUsersToProjectRequest
            {
                ProjectId = Guid.NewGuid(),
                Users = new List<ProjectUserRequest>
                {

                    new ProjectUserRequest
                    {
                        RoleId = Guid.NewGuid(),
                        User = new UserRequest
                        {
                            Id = _dbProjectUsers.ElementAt(0).UserId,
                            IsActive = true
                        }
                    }
                }
            };

            validator.TestValidate(newRequest).ShouldNotHaveAnyValidationErrors();
            _repository.Verify();
        }

        [Test]
        public void ShouldNotHaveAnyValidationErrorsWhenRequestIsValid()
        {
            validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
        }
    }
}
