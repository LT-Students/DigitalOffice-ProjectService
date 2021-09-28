using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Business.Commands.UnitTests
{
  class FindTaskPropertyCommandTests
    {
        private Guid _projectId;
        private List<DbTaskProperty> _dbTaskProperties;
        private List<TaskPropertyInfo> _taskPropertiesInfo;

        private AutoMocker _mocker;
        private IFindTaskPropertyCommand _command;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectId = Guid.NewGuid();

            _mocker = new();
            _taskPropertiesInfo = new();

            _command = _mocker.CreateInstance<FindTaskPropertyCommand>();

            _dbTaskProperties = new List<DbTaskProperty>
            {
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = "Feature",
                    ProjectId = _projectId,
                    CreatedBy = Guid.NewGuid(),
                    PropertyType = (int)TaskPropertyType.Type,
                    Description = "Description",
                    CreatedAtUtc = DateTime.UtcNow,
                    IsActive = true
                },
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = "Feature",
                    ProjectId = _projectId,
                    CreatedBy = Guid.NewGuid(),
                    PropertyType = (int)TaskPropertyType.Type,
                    Description = "Description",
                    CreatedAtUtc = DateTime.UtcNow,
                    IsActive = true
                },
                new DbTaskProperty
                {
                    Id = Guid.NewGuid(),
                    Name = "Feature",
                    ProjectId = _projectId,
                    CreatedBy = Guid.NewGuid(),
                    PropertyType = (int)TaskPropertyType.Type,
                    Description = "Description",
                    CreatedAtUtc = DateTime.UtcNow,
                    IsActive = true
                }
            };

            foreach (var dbTaskProperty in _dbTaskProperties)
            {
                _taskPropertiesInfo.Add(
                    new TaskPropertyInfo
                    {
                        Id = dbTaskProperty.Id,
                        ProjectId = dbTaskProperty.ProjectId,
                        CreatedBy = dbTaskProperty.CreatedBy,
                        Name = dbTaskProperty.Name,
                        CreatedAtUtc = dbTaskProperty.CreatedAtUtc,
                        Description = dbTaskProperty.Description,
                        IsActive = dbTaskProperty.IsActive,
                        PropertyType = (TaskPropertyType)dbTaskProperty.PropertyType
                    });
            }
        }

        [Test]
        public void ShouldReturnUsersByNameSuccessful()
        {
            int skipCount = 0;
            int totalCount = _dbTaskProperties.Count;
            int takeCount = _dbTaskProperties.Count;

            var filter = new FindTaskPropertiesFilter
            {
                Name = "Feature"
            };

            var result = new FindResponse<TaskPropertyInfo>
            {
                Body = _taskPropertiesInfo,
                TotalCount = totalCount
            };

            _mocker.Setup<ITaskPropertyRepository, IEnumerable<DbTaskProperty>>(x =>
                x.Find(filter, skipCount, takeCount, out totalCount))
                .Returns(_dbTaskProperties);

            _mocker.SetupSequence<ITaskPropertyInfoMapper, TaskPropertyInfo>(x =>
                x.Map(It.IsAny<DbTaskProperty>()))
                .Returns(_taskPropertiesInfo[0])
                .Returns(_taskPropertiesInfo[1])
                .Returns(_taskPropertiesInfo[2]);

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<ITaskPropertyRepository, IEnumerable<DbTaskProperty>>(x =>
                x.Find(filter, skipCount, takeCount, out totalCount), Times.Once);
            _mocker.Verify<ITaskPropertyInfoMapper, TaskPropertyInfo>(x =>
                x.Map(It.IsAny<DbTaskProperty>()), Times.Exactly(_taskPropertiesInfo.Count));
        }
    }
}
