using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.UnitTests
{
    internal class ProjectUserRequestMapperTests
    {
        private IProjectUserRequestMapper _projectUserRequestMapper;

        private ProjectUser _projectUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectUserRequestMapper = new ProjectUserRequestMapper();

            _projectUser = new ProjectUser
            {
                User = new User
                {
                    Id = Guid.NewGuid()
                },

                Role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Manager"
                }
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenProjectUserRequestIsNull()
        {
            ProjectUser projectRequest = null;

            var expectedDbProjectUser = new DbProjectUser
            {
                UserId = _projectUser.User.Id,
                RoleId = _projectUser.Role.Id,
            };

            Assert.Throws<ArgumentNullException>(() => _projectUserRequestMapper.Map(projectRequest));
        }

        [Test]
        public void ShouldReturnDbProjectWhenProjectUserRequestIsMapped()
        {
            var expectedDbProjectUser = new DbProjectUser
            {
                UserId = _projectUser.User.Id,
                RoleId = _projectUser.Role.Id,
            };

            var dbProjectUser = _projectUserRequestMapper.Map(_projectUser);

            SerializerAssert.AreEqual(expectedDbProjectUser, dbProjectUser);
        }
    }
}
