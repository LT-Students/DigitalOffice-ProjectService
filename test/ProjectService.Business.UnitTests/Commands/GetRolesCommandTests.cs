using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.UnitTests.Commands
{
    class GetRolesCommandTests
    {
        private IGetRolesCommand _command;

        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IRolesResponseMapper> _mapperMock;

        private RolesResponse _rolesResponse;
        private List<Guid> _rolesIds;
        private int _skip;
        private int _take;
        private int _totalCount;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _skip = 0;
            _take = 3;
            _totalCount = 3;

            var roles = new List<Role>();
            _rolesIds = new List<Guid>();

            for (int i=0; i <3; i++)
            {
                _rolesIds.Add(Guid.NewGuid());

                roles.Add(
                    new Role
                    {
                        Id = _rolesIds[i],
                        Name = "Role name test",
                        Description = "Role description test"
                    });
            }

            _rolesResponse = new RolesResponse
            {
                Roles = roles,
                TotalCount = _totalCount
            };
        }

        [SetUp]
        public void SetUp()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IRolesResponseMapper>();
            _command = new GetRolesCommand(_roleRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsIt()
        {
            _roleRepositoryMock
                .Setup(r => r.GetRoles(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<DbRole>());

            _mapperMock
                .Setup(m => m.Map(It.IsAny<List<DbRole>>()))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _command.Execute(_skip, _take));
            _roleRepositoryMock.Verify(repository => repository.GetRoles(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<List<DbRole>>()), Times.Once);
        }

        [Test]
        public void ShouldReturnRolesInfo()
        {
            var expectedRoles = new List<Role>();
            
            for (int i = 0; i < 3; i++)
            {
                expectedRoles.Add(
                    new Role
                    {
                        Id = _rolesIds[i],
                        Name = "Role name test",
                        Description = "Role description test"
                    });
            }

            _roleRepositoryMock
                .Setup(r => r.GetRoles(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<DbRole>());

            _mapperMock
                .Setup(m => m.Map(It.IsAny<List<DbRole>>()))
                .Returns(_rolesResponse);

            var result = _command.Execute(_skip, _take);

            SerializerAssert.AreEqual(expectedRoles, result.Roles);
        }
    }
}
