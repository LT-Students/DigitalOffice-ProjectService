//using LT.DigitalOffice.ProjectService.Data;
//using LT.DigitalOffice.ProjectService.Data.Interfaces;
//using LT.DigitalOffice.ProjectService.Data.Provider;
//using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
//using LT.DigitalOffice.ProjectService.Models.Db;
//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
//{
//    internal class GetProjectUsersRepositoryTests
//    {
//        private IDataProvider _provider;
//        private IUserRepository _repository;

//        private Guid _projectId;
//        private DbProjectUser _activeDbUser;
//        private DbProjectUser _notActiveDbUser;

//        [SetUp]
//        public void SetUp()
//        {
//            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
//                                    .UseInMemoryDatabase("InMemoryDatabase")
//                                    .Options;

//            _provider = new ProjectServiceDbContext(dbOptions);
//            _repository = new UserRepository(_provider);

//            _projectId = Guid.NewGuid();

//            _activeDbUser = new DbProjectUser
//            {
//                Id = Guid.NewGuid(),
//                UserId = Guid.NewGuid(),
//                ProjectId = _projectId,
//                IsActive = true
//            };

//            _notActiveDbUser = new DbProjectUser
//            {
//                Id = Guid.NewGuid(),
//                UserId = Guid.NewGuid(),
//                ProjectId = _projectId,
//                IsActive = false
//            };

//            _provider.ProjectsUsers.AddRange(_activeDbUser, _notActiveDbUser);
//            _provider.Save();
//        }

//        [TearDown]
//        public void Clean()
//        {
//            if (_provider.IsInMemory())
//            {
//                _provider.EnsureDeleted();
//            }
//        }

//        [Test]
//        public void ShouldReturnEmptyListWhenProjectDoesNotExist()
//        {
//            var result = _repository.GetAsync(Guid.NewGuid(), true);

//            var expected = new List<DbProjectUser> {};

//            Assert.AreEqual(expected.Count(), result.Count());
//        }

//        [Test]
//        public void ShouldReturnAllDbProjectUsers()
//        {
//            var result = _repository.GetAsync(_projectId, true);

//            var expected = new List<DbProjectUser> { _activeDbUser, _notActiveDbUser };

//            Assert.AreEqual(expected.Count(), result.Count());

//            Assert.NotNull(result.First(x => x.Id == _activeDbUser.Id));
//            Assert.NotNull(result.First(x => x.Id == _notActiveDbUser.Id));
//        }

//        [Test]
//        public void ShouldReturnOnlyActiveDbProjectUsers()
//        {
//            var result = _repository.GetAsync(_projectId, false);

//            var expected = new List<DbProjectUser> { _activeDbUser };

//            Assert.AreEqual(expected.Count(), result.Count());

//            Assert.NotNull(result.First(x => x.Id == _activeDbUser.Id));
//        }
//    }
//}
