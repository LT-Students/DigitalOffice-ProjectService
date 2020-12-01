using LT.DigitalOffice.ProjectService.Business.Commands;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
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

        private List<DbRole> _dbRoles;
        private int _skip;
        private int _take;
        private int _totalCount;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _skip = 0;
            _take = 3;
            _totalCount = 3;

            _dbRoles = new List<DbRole>();

            for (int i=0; i <3; i++)
            {
                _dbRoles.Add(
                    new DbRole
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Role name test {i}",
                        Description = $"Role description test {i}"
                    });
            }
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
                        Id = _dbRoles[i].Id,
                        Name = _dbRoles[i].Name,
                        Description = _dbRoles[i].Description
                    });
            }

            var expected = new RolesResponse
            {
                Roles = expectedRoles,
                TotalCount = _totalCount
            };

            _roleRepositoryMock
                .Setup(r => r.GetRoles(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<DbRole>());

            _mapperMock
                .Setup(m => m.Map(It.IsAny<List<DbRole>>()))
                .Returns(expected);

            var result = _command.Execute(_skip, _take);

            SerializerAssert.AreEqual(expected, result);
            _roleRepositoryMock.Verify(repository => repository.GetRoles(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map(It.IsAny<List<DbRole>>()), Times.Once);
        }
    }
}
