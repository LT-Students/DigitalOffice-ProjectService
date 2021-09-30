using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
    public class TaskPropertyRepositoryTests
    {
        private IDataProvider _provider;
        private ITaskPropertyRepository _repository;

        private readonly Guid _taskPropertyId = Guid.NewGuid();
        private readonly string _name = "Name";
        private readonly string _description = "Description";
        private readonly int _type = (int)TaskPropertyType.Status;
        private Mock<IHttpContextAccessor> _accessorMock;

    private List<DbTaskProperty> _dbTaskProperties;

        private void CreateInMemoryDb()
        {
            _accessorMock = new();
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptions);

            _provider.TaskProperties.AddRange(_dbTaskProperties);

            _provider.Save();
            _repository = new TaskPropertyRepository(_provider, _accessorMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _dbTaskProperties = new List<DbTaskProperty>()
            {
                new DbTaskProperty
                {
                    Id = _taskPropertyId,
                    ProjectId = Guid.NewGuid(),
                    Name = _name,
                    Description = _description,
                    PropertyType = _type
                },
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    ProjectId = Guid.NewGuid(),
                    Name = _name,
                    Description = _description,
                    PropertyType = _type
                },
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    ProjectId = Guid.NewGuid(),
                    Name = _name,
                    Description = _description,
                    PropertyType = _type
                }
            };

            CreateInMemoryDb();
        }

        [Test]
        public void AreExist()
        {
            Assert.IsTrue(_repository.AreExist(_taskPropertyId));
        }

        [Test]
        public void Get()
        {
            SerializerAssert.AreEqual(_dbTaskProperties[0], _repository.Get(_taskPropertyId));
        }

        [Test]
        public void ShouldReturnTrueWhenTaskPropertiesExist()
        {
            string[] propertyNames = new string [3] { "Name", "Feature", "Bug" };

            Assert.IsTrue(_repository.AreExistForProject(_dbTaskProperties[0].ProjectId.Value, propertyNames));
        }

        [Test]
        public void ShouldReturnTrueWhenTaskPropertiesDoesNotExist()
        {
            string[] propertyNames = new string[2] { "Feature", "Bug" };

            Assert.IsFalse(_repository.AreExistForProject(_dbTaskProperties[0].ProjectId.Value, propertyNames));
        }

        [Test]
        public void CreateTaskPropertySuccessful()
        {
            var dbTaskProperties = new List<DbTaskProperty>()
            {
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = _name,
                    Description = _description,
                    PropertyType = _type
                },
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = _name,
                    Description = _description,
                    PropertyType = _type
                }
            };

            _repository.Create(dbTaskProperties);
            var dbIds = dbTaskProperties.Select(x => x.Id);

           SerializerAssert.AreEqual(dbTaskProperties, _provider.TaskProperties.Where(x => dbIds.Contains(x.Id)).Reverse());
        }

        #region Find
        /*[Test]
        public void ShouldDbTaskPropertiesReturnByNameSuccessful()
        {
            var filter = new FindTaskPropertiesFilter
            {
                Name = "Name"
            };

            SerializerAssert.AreEqual(_dbTaskProperties, _repository.Find(filter, 0, _dbTaskProperties.Count, out int totalCount));
            Assert.AreEqual(_dbTaskProperties.Count, totalCount);
        }

        [Test]
        public void ShouldDbTaskPropertiesReturnByShortNameSuccessful()
        {
            var filter = new FindTaskPropertiesFilter
            {
                Name = "Nam"
            };

            SerializerAssert.AreEqual(_dbTaskProperties, _repository.Find(filter, 0, _dbTaskProperties.Count, out int totalCount));
            Assert.AreEqual(_dbTaskProperties.Count, totalCount);
        }

        [Test]
        public void ShouldDbTaskPropertiesReturnByProjectIdSuccessful()
        {
            var filter = new FindTaskPropertiesFilter
            {
                ProjectId = _dbTaskProperties[0].ProjectId
            };

            var expectedResult = new List<DbTaskProperty> { _dbTaskProperties[0] };

            SerializerAssert.AreEqual(expectedResult, _repository.Find(filter, 0, 1, out int totalCount));
            Assert.AreEqual(expectedResult.Count, totalCount);
        }

        [Test]
        public void ShouldDbTaskPropertiesReturnByAuthorIdSuccessful()
        {
            var filter = new FindTaskPropertiesFilter
            {
                AuthorId = _dbTaskProperties[0].AuthorId
            };

            var expectedResult = new List<DbTaskProperty> { _dbTaskProperties[0] };

            SerializerAssert.AreEqual(expectedResult, _repository.Find(filter, 0, 1, out int totalCount));
            Assert.AreEqual(_dbTaskProperties.Count, totalCount);
        }*/

        #endregion

        [TearDown]
        public void CleanDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }
    }
}
