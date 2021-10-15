using LT.DigitalOffice.ProjectService.Mappers.Models;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
{
    class ProjectFileInfoMapperTests
    {
        public DbProjectFile _dbProjectFile;
        public ProjectFileInfo _expectedProjectFileInfo;

        public IProjectFileInfoMapper _mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new ProjectFileInfoMapper();

            _dbProjectFile = new DbProjectFile
            {
                Id = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid()
            };

            _expectedProjectFileInfo = new ProjectFileInfo
            {
                ProjectId = _dbProjectFile.ProjectId,
                FileId = _dbProjectFile.FileId
            };
        }

        [Test]
        public void ShouldThrowExceptionWhenDbProjectFileIsNull()
        {
            Assert.IsNull(_mapper.Map(null));
        }

        [Test]
        public void ShouldReturnProjectFileInfoSuccessful()
        {
            SerializerAssert.AreEqual(_expectedProjectFileInfo, _mapper.Map(_dbProjectFile));
        }
    }
}
