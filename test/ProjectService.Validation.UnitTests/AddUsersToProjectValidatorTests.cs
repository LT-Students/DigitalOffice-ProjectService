﻿using FluentValidation.TestHelper;
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

        private Mock<IUserRepository> _repository;

        private AddUsersToProjectRequest _request;
        private IEnumerable<DbProjectUser> _dbProjectUsers;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IUserRepository>();
            validator = new AddUsersToProjectValidator(_repository.Object);

            var projectId = Guid.NewGuid();

            _dbProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    Role = (int)UserRoleType.ProjectAdmin,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    UserId = Guid.NewGuid(),
                    Role = (int)UserRoleType.ProjectAdmin,
                    IsActive = true
                }
            };

            var projectUsers = new List<ProjectUserRequest>
            {
                new ProjectUserRequest
                {
                    Role = (int)UserRoleType.ProjectAdmin,
                    UserId = Guid.NewGuid()
                },
                new ProjectUserRequest
                {
                    Role = (int)UserRoleType.ProjectAdmin,
                    UserId = Guid.NewGuid()
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
                        Role = (int)UserRoleType.ProjectAdmin,
                        UserId = _dbProjectUsers.ElementAt(0).UserId
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
