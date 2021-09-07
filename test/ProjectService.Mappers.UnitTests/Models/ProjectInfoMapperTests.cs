//using LT.DigitalOffice.Models.Broker.Responses.Company;
//using LT.DigitalOffice.ProjectService.Mappers.Models;
//using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
//using LT.DigitalOffice.ProjectService.Models.Db;
//using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
//using LT.DigitalOffice.ProjectService.Models.Dto.Models;
//using LT.DigitalOffice.UnitTestKernel;
//using Moq;
//using NUnit.Framework;
//using System;

//namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
//{
//    class ProjectInfoMapperTests
//    {
//        public DbProject _dbProject;
//        public ProjectInfo _expectedProjectInfo;

//        public IProjectInfoMapper _mapper;
//        public Mock<IGetDepartmentResponse> _department;

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _mapper = new ProjectInfoMapper();

//            _dbProject = new DbProject
//            {
//                Id = Guid.NewGuid(),
//                CreatedBy = Guid.NewGuid(),
//                Name = "Project for Lanit-Tercom",
//                ShortName = "Project",
//                Description = "New project for Lanit-Tercom",
//                ShortDescription = "Short description",
//                DepartmentId = Guid.NewGuid(),
//                CreatedAtUtc = DateTime.UtcNow,
//                Status = (int)ProjectStatusType.Active
//            };

//            _expectedProjectInfo = new ProjectInfo
//            {
//                Id = _dbProject.Id,
//                CreatedBy = _dbProject.CreatedBy,
//                Name = "Project for Lanit-Tercom",
//                ShortName = "Project",
//                Description = "New project for Lanit-Tercom",
//                ShortDescription = "Short description",
//                CreatedAtUtc = _dbProject.CreatedAtUtc,
//                Status = ProjectStatusType.Active,
//                Department = new DepartmentInfo
//                {
//                    Id = _dbProject.DepartmentId.Value,
//                    Name = "Some department"
//               }
//            };

//            _department = new Mock<IGetDepartmentResponse>();
//            _department.Setup(x => x.DepartmentId).Returns(_dbProject.DepartmentId.Value);
//            _department.Setup(x => x.Name).Returns("Some department");
//        }

//        [Test]
//        public void ShouldThrowExceptionWhenDbProjectIsNull()
//        {
//            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, _department.Object.Name));
//        }

//        [Test]
//        public void ShouldReturnProjectInfoSuccessful()
//        {
//            SerializerAssert.AreEqual(_expectedProjectInfo, _mapper.Map(_dbProject, _department.Object.Name));
//        }
//    }
//}
