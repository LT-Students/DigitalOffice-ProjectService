using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.UnitTests
{
    internal class ProjectUserRequestMapperTests
    {
        private IProjectUserRequestMapper _projectUserRequestMapper;

        private ProjectUserRequest _projectUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectUserRequestMapper = new ProjectUserRequestMapper();

            _projectUser = new ProjectUserRequest
            {
                UserId = Guid.NewGuid(),
                Role = ProjectUserRoleType.Admin
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenProjectUserRequestIsNull()
        {
            ProjectUserRequest projectRequest = null;

            Assert.Throws<ArgumentNullException>(() => _projectUserRequestMapper.Map(projectRequest));
        }

        [Test]
        public void ShouldReturnDbProjectWhenProjectUserRequestIsMapped()
        {
            var dbProjectUser = _projectUserRequestMapper.Map(_projectUser);

            var expectedDbProjectUser = new DbProjectUser
            {
                Id = dbProjectUser.Id,
                UserId = _projectUser.UserId,
                Role = (int)_projectUser.Role,
                AddedOn = dbProjectUser.AddedOn,
                IsActive = true
            };

            SerializerAssert.AreEqual(expectedDbProjectUser, dbProjectUser);
        }
    }
}
