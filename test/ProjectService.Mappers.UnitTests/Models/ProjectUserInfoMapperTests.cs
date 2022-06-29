using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
{
  class ProjectUserInfoMapperTests
    {
        public DbProjectUser _dbProjectUser;
        public UserInfo _expectedProjectUserInfo;
        public UserData _userData;

        public IUserInfoMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new UserInfoMapper();

            _userData = new UserData(
                id: Guid.NewGuid(),
                firstName: "Spartak",
                lastName: "Ryabtsev",
                middleName: "Alexandrovich",
                isActive: true,
                imageId: null);

            _dbProjectUser = new DbProjectUser
            {
                Id = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                UserId = _userData.Id,
                IsActive = _userData.IsActive
            };

            _expectedProjectUserInfo = new UserInfo
            {
                Id = _userData.Id,
                FirstName = _userData.FirstName,
                LastName = _userData.LastName,
                MiddleName = _userData.MiddleName,
                IsActive = _userData.IsActive
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenDbProjectUserIsNull()
        {
            Assert.Null(_mapper.Map(null, null, null, null));
        }

        //[Test]
        //public void ShouldReturnProjectUserInfoSuccessful()
        //{
        //    SerializerAssert.AreEqual(_expectedProjectUserInfo, _mapper.Map(_userData, _dbProjectUser));
        //}
    }
}
