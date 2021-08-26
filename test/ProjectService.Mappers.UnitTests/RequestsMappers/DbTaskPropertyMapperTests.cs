using LT.DigitalOffice.ProjectService.Mappers.Db;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.RequestsMappers
{
    class DbTaskPropertyMapperTests
    {
        private Guid _authorId;
        private Guid _projectId;
        private TaskProperty _taskProperty;
        private DbTaskProperty _expectedDbTaskProperty;

        private IDbTaskPropertyMapper _mapper;

        [OneTimeSetUp]
        public void OneTImeSetUp()
        {
            _authorId = Guid.NewGuid();
            _projectId = Guid.NewGuid();

            _mapper = new DbTaskPropertyMapper();

            _taskProperty = new TaskProperty
            {
                Name = "Bug",
                PropertyType = TaskPropertyType.Type,
                Description = "Description"
            };

            _expectedDbTaskProperty = new DbTaskProperty
            {
                ProjectId = _projectId,
                CreatedBy = _authorId,
                Name = _taskProperty.Name,
                IsActive = true,
                Description = _taskProperty.Description,
                PropertyType = (int)_taskProperty.PropertyType,
            };
        }

        [Test]
        public void ShouldReturnDbTaskPropertySuccessful()
        {
            var result = _mapper.Map(_taskProperty, _authorId, _projectId);
            _expectedDbTaskProperty.CreatedBy = result.CreatedBy;
            _expectedDbTaskProperty.CreatedAtUtc = result.CreatedAtUtc;
            _expectedDbTaskProperty.Id = result.Id;

            SerializerAssert.AreEqual(_expectedDbTaskProperty, result);
        }
    }
}
