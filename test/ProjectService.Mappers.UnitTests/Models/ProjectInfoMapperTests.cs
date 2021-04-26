using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
{
    class ProjectInfoMapperTests
    {
        public DbProject _dbProject;
        public ProjectInfo _expectedProjectInfo;

        public IProjectInfoMapper _mapper;
        public Mock<IGetDepartmentResponse> _department;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapper = new ProjectInfoMapper();

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                DepartmentId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = (int)ProjectStatusType.Active
            };

            _expectedProjectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                AuthorId = _dbProject.AuthorId,
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                CreatedAt = _dbProject.CreatedAt,
                Status = ProjectStatusType.Active,
                Department = new DepartmentInfo
                {
                    Id = _dbProject.DepartmentId,
                    Name = "Some department"
               }
            };

            _department = new Mock<IGetDepartmentResponse>();
            _department.Setup(x => x.Id).Returns(_dbProject.DepartmentId);
            _department.Setup(x => x.Name).Returns("Some department");
        }

        [Test]
        public void ShouldThrowExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, _department.Object.Name));
        }

        [Test]
        public void ShouldReturnProjectInfoSuccessful()
        {
            SerializerAssert.AreEqual(_expectedProjectInfo, _mapper.Map(_dbProject, _department.Object.Name));
        }
    }
}
