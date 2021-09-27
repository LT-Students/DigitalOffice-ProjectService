using LT.DigitalOffice.ProjectService.Mappers.Db;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.UnitTests
{
    internal class ProjectUserRequestMapperTests
    {
        private ProjectUserRequest _projectUser;
        private IDbProjectUserMapper _projectUserRequestMapper;
        private Mock<IHttpContextAccessor> _accessorMock;

        private Guid _authorId = Guid.NewGuid();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _accessorMock = new();
            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _authorId);

            _accessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(_items);

            _projectUserRequestMapper = new DbProjectUserMapper(_accessorMock.Object);

            _projectUser = new ProjectUserRequest
            {
                UserId = Guid.NewGuid(),
                Role = ProjectUserRoleType.Manager
            };
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
                CreatedAtUtc = dbProjectUser.CreatedAtUtc,
                CreatedBy = dbProjectUser.CreatedBy,
                IsActive = true
            };

            SerializerAssert.AreEqual(expectedDbProjectUser, dbProjectUser);
        }
    }
}
