using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.ResponseMappers
{
    class RolesResponseMapperTests
    {
        private IRolesResponseMapper _rolesResponseMapper;

        private List<DbRole> _dbRoles;

        private int _totalCount;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var roleMapper = new RoleMapper();
            _totalCount = 3;
            _rolesResponseMapper = new RolesResponseMapper(roleMapper);

            _dbRoles = new List<DbRole>();

            for (int i = 0; i < _totalCount; i++)
            {
                _dbRoles.Add(
                    new DbRole
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Test role name {i}",
                        Description = $"Test role description {i}"
                    });
            }
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _rolesResponseMapper.Map(null));
        }

        [Test]
        public void ShouldReturnRolesResponseModelWhenDbRolesIsMapped()
        {
            var result = _rolesResponseMapper.Map(_dbRoles);

            var expected = new RolesResponse
            {
                Roles = new List<Role>
                {
                    new Role
                    {
                        Id = _dbRoles.ElementAt(0).Id,
                        Name = _dbRoles.ElementAt(0).Name,
                        Description = _dbRoles.ElementAt(0).Description
                    },
                    new Role
                    {
                        Id = _dbRoles.ElementAt(1).Id,
                        Name = _dbRoles.ElementAt(1).Name,
                        Description = _dbRoles.ElementAt(1).Description
                    },
                    new Role
                    {
                        Id = _dbRoles.ElementAt(2).Id,
                        Name = _dbRoles.ElementAt(2).Name,
                        Description = _dbRoles.ElementAt(2).Description
                    },
                },
                TotalCount = _totalCount
            };

            SerializerAssert.AreEqual(expected, result);
        }
    }
}
