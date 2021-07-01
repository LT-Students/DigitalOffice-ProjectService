using System;
using LT.DigitalOffice.ProjectService.Mappers.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
{
    public class TaskPropertyInfoMapperTests
    {
        private ITaskPropertyInfoMapper _mapper;

        private DbTaskProperty _request;
        private TaskPropertyInfo _expectedResult;

        private Guid _id = Guid.NewGuid();
        private Guid _authorId = Guid.NewGuid();
        private Guid _projectId = Guid.NewGuid();
        private int _propertyType = 1;
        private string _name = "Name";
        private string _description = "Description";
        private DateTime _createdAt = DateTime.Now;
        private bool _isActive = true; 
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new TaskPropertyInfoMapper();
        }
        
        [SetUp]
        public void SetUp()
        {
            _request = new DbTaskProperty()
            {
                Id = _id,
                AuthorId = _authorId,
                ProjectId = _projectId,
                PropertyType = _propertyType,
                Name = _name,
                Description = _description,
                CreatedAt = _createdAt,
                IsActive = _isActive
            };

            _expectedResult = new TaskPropertyInfo()
            {
                Id = _id,
                AuthorId = _authorId,
                ProjectId = _projectId,
                PropertyType = _propertyType,
                Name = _name,
                Description = _description,
                CreatedAt = _createdAt,
                IsActive = _isActive
            };
        }

        [Test]
        public void ShouldReturnSuccessfulModel()
        {
            SerializerAssert.AreEqual(_expectedResult, _mapper.Map(_request));
        }

        [Test]
        public void ExceptionWhenRequestIsNull()
        {
            _request = null;

            Assert.Throws<ArgumentNullException>(() => _mapper.Map(_request));
        }
    }
}