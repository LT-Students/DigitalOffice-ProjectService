using System;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
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
        private readonly int _type = 1;

        private DbTaskProperty _dbTaskProperty;

        private void CreateInMemoryDb()
        {
            var dbOptions = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _provider = new ProjectServiceDbContext(dbOptions);

            _dbTaskProperty = new DbTaskProperty()
            {
                Id = _taskPropertyId,
                Description = _description,
                Name = _name,
                PropertyType = _type
            };

            _provider.TaskProperties.Add(_dbTaskProperty);

            _provider.Save();
            _repository = new TaskPropertyRepository(_provider);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _dbTaskProperty = new DbTaskProperty()
            {
                Id = _taskPropertyId,
                Name = _name,
                Description = _description,
                PropertyType = _type
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
            SerializerAssert.AreEqual(_dbTaskProperty, _repository.Get(_taskPropertyId));
        }

        [OneTimeTearDown]
        public void CleanDb()
        {
            if (_provider.IsInMemory())
            {
                _provider.EnsureDeleted();
            }
        }
    }
}
