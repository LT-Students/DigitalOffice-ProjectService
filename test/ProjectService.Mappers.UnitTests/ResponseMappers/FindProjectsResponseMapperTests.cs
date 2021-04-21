using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers;
using LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.ResponseMappers
{
    class FindProjectsResponseMapperTests
    {
        private Mock<IProjectInfoMapper> _projectInfoMapperMock;

        private IFindProjectsResponseMapper _mapper;

        private List<DbProject> _dbProjects;
        private List<string> errors = new();
        private string _departmentName;
        private DbProject _dbProject;
        private ProjectInfo _projectInfo;
        private ProjectsResponse _response;

        private const int _totalCount = 1;

        [SetUp]
        public void SetUp()
        {
            _departmentName = "departmentName";

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                ShortName = "SH",
                ShortDescription = "short description",
                DepartmentId = Guid.NewGuid()
            };

            _dbProjects = new List<DbProject> { _dbProject };

            _projectInfo = new ProjectInfo
            {
                Id = _dbProject.Id,
                Name = "Name",
                ShortName = "SH",
                ShortDescription = "short description",
                DepartmentInfo = new DepartmentInfo
                {
                    Id = _dbProject.DepartmentId,
                    Name = _departmentName
                }
            };

            _response = new ProjectsResponse
            {
                TotalCount = _totalCount,
                Projects = new List<ProjectInfo> { _projectInfo },
                Errors = errors
            };

            _projectInfoMapperMock = new Mock<IProjectInfoMapper>();
            _projectInfoMapperMock
                .Setup(x => x.Map(It.IsAny<DbProject>(), It.IsAny<string>()))
                .Returns(_projectInfo);

            _mapper = new FindProjectsResponseMapper(_projectInfoMapperMock.Object);
        }


        [Test]
        public void ShouldReturnProjectsResponse()
        {
            SerializerAssert.AreEqual(_response, _mapper.Map(_dbProjects, _totalCount, _departmentName, errors));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenListOfDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, _totalCount, _departmentName, errors));
        }
    }
}
