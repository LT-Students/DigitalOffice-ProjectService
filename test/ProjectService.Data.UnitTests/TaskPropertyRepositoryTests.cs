using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
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

        private List<DbTaskProperty> _dbTaskProperties;

        private void CreateInMemoryDb()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptions);

/*            _dbTaskProperties = new DbTaskProperty()
            {
                Id = _taskPropertyId,
                Description = _description,
                Name = _name,
                PropertyType = _type
            };
*/
            _provider.TaskProperties.AddRange(_dbTaskProperties);

            _provider.Save();
            _repository = new TaskPropertyRepository(_provider);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _dbTaskProperties = new List<DbTaskProperty>()
            {
                new DbTaskProperty
                {
                    Id = _taskPropertyId,
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
                },
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
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

        #region Find
        [Test]
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
            Assert.AreEqual(_dbTaskProperties.Count, totalCount);
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
        }

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
