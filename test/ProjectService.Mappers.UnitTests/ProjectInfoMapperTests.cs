using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests
{
    class ProjectInfoMapperTests
    {
        private ProjectInfoMapper _mapper;

        private DbProject _dbProject;
        private ProjectInfo _projectInfo;

        private const string DepartmentName = "department name";

        [SetUp]
        public void SetUp()
        {
            _mapper = new ProjectInfoMapper();

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                ShortName = "SH",
                ShortDescription = "short description",
                DepartmentId = Guid.NewGuid()
            };

            _projectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                Name = "Name",
                ShortName = "SH",
                ShortDescription = "short description",
                Department = new DepartmentInfo
                {
                    Id = _dbProject.DepartmentId,
                    Name = DepartmentName
                }
            };
        }

        [Test]
        public void ShouldReturnProjectsInfo()
        {
            SerializerAssert.AreEqual(_projectInfo, _mapper.Map(_dbProject, DepartmentName));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, DepartmentName));
        }
    }
}
