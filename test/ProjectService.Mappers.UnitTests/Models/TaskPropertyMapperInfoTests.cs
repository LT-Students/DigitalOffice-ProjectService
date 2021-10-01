using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Mappers.Models.UnitTests
{
    class TaskPropertyMapperInfoTests
    {
        private DbTaskProperty _dbTaskProperty;
        private TaskPropertyInfo _taskPropertyInfo;

        private ITaskPropertyInfoMapper _mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            _mapper = new TaskPropertyInfoMapper();

            _dbTaskProperty = new DbTaskProperty
            {
                Id = Guid.NewGuid(),
                Name = "Feature",
                ProjectId = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
                PropertyType = (int)TaskPropertyType.Type,
                Description = "Description",
                CreatedAtUtc = DateTime.UtcNow,
                IsActive = true
            };

            _taskPropertyInfo = new TaskPropertyInfo
            {
                Id = _dbTaskProperty.Id,
                Name = _dbTaskProperty.Name,
                ProjectId = _dbTaskProperty.ProjectId,
                CreatedBy = _dbTaskProperty.CreatedBy,
                PropertyType = (TaskPropertyType)_dbTaskProperty.PropertyType,
                Description = _dbTaskProperty.Description,
                CreatedAtUtc = _dbTaskProperty.CreatedAtUtc,
                IsActive = _dbTaskProperty.IsActive
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenDbTaskPropertyIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
        }

        [Test]
        public void ShouldTaskPropertyInfoReturnSuccessful()
        {
            SerializerAssert.AreEqual(_taskPropertyInfo, _mapper.Map(_dbTaskProperty));
        }
    }
}
