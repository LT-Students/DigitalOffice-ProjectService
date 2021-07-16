using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
{
    class ProjectUserInfoMapperTests
    {
        public DbProjectUser _dbProjectUser;
        public ProjectUserInfo _expectedProjectUserInfo;
        public UserData _userData;

        public IProjectUserInfoMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new ProjectUserInfoMapper();

            _userData = new UserData(
                id: Guid.NewGuid(),
                firstName: "Spartak",
                lastName: "Ryabtsev",
                middleName: "Alexandrovich",
                isActive: true,
                imageId: null,
                rate: null);

            _dbProjectUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                UserId = _userData.Id,
                IsActive = _userData.IsActive,
                AddedOn = DateTime.Now,
                RemovedOn = DateTime.Now
            };

            _expectedProjectUserInfo = new ProjectUserInfo
            {
                Id = _userData.Id,
                FirstName = _userData.FirstName,
                LastName = _userData.LastName,
                MiddleName = _userData.MiddleName,
                IsActive = _userData.IsActive,
                AddedOn = _dbProjectUser.AddedOn,
                RemovedOn = _dbProjectUser.RemovedOn,
                Role = UserRoleType.ProjectAdmin.ToString()
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenDbProjectUserIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, null));
        }

        [Test]
        public void ShouldReturnProjectUserInfoSuccessful()
        {
            SerializerAssert.AreEqual(_expectedProjectUserInfo, _mapper.Map(_userData, _dbProjectUser));
        }
    }
}
