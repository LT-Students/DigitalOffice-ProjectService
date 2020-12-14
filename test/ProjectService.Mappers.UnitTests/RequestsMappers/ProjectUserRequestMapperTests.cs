using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.RequestsMappers
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
                User = new UserRequest
                {
                    Id = Guid.NewGuid()
                },
                RoleId = Guid.NewGuid()
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenProjectUserRequestIsNull()
        {
            ProjectUserRequest projectRequest = null;

            var expectedDbProjectUser = new DbProjectUser
            {
                UserId = _projectUser.User.Id,
                RoleId = _projectUser.RoleId,
            };

            Assert.Throws<ArgumentNullException>(() => _projectUserRequestMapper.Map(projectRequest));
        }

        [Test]
        public void ShouldReturnDbProjectWhenProjectUserRequestIsMapped()
        {
            var dbProjectUser = _projectUserRequestMapper.Map(_projectUser);

            var expectedDbProjectUser = new DbProjectUser
            {
                UserId = _projectUser.User.Id,
                RoleId = _projectUser.RoleId,
                AddedOn = dbProjectUser.AddedOn
            };

            SerializerAssert.AreEqual(expectedDbProjectUser, dbProjectUser);
        }
    }
}
