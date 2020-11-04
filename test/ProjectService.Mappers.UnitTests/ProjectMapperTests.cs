using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Mappers
{
    public class ProjectMapperTests
    {
        private const string Name = "Test Project";
        private const string Description = "DigitalOffice project. The students do the work. Sometimes. Never (c) Spartak";
        private const string ShortName = "TP";
        private Guid projectId;
        private DateTime createdAt;
        private Guid departmentId;

        private IMapper<DbProject, Project> dbProjectMapper;
        private IMapper<NewProjectRequest, DbProject> newProjectRequestMapper;
        private IMapper<EditProjectRequest, DbProject> editProjectRequestMapper;

        private DbProject dbProject;
        private NewProjectRequest newProjectRequest;
        private EditProjectRequest editProjectRequest;

        [SetUp]
        public void SetUp()
        {
            dbProjectMapper = new ProjectMapper();
            newProjectRequestMapper = new ProjectMapper();
            editProjectRequestMapper = new ProjectMapper();

            projectId = Guid.NewGuid();
            createdAt = DateTime.Now;
            departmentId = Guid.NewGuid();

            dbProject = new DbProject
            {
                Id = projectId,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                CreatedAt = createdAt,
                IsActive = false
            };

            newProjectRequest = new NewProjectRequest
            {
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                DepartmentId = departmentId,
                IsActive = true
            };

            editProjectRequest = new EditProjectRequest
            {
                Id = projectId,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                DepartmentId = departmentId,
                IsActive = true
            };
        }

        #region IMapper<DbProject, Project>
        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => dbProjectMapper.Map(null));
        }

        [Test]
        public void ShouldReturnProjectModelWhenDbProjectIsMapped()
        {
            var result = dbProjectMapper.Map(dbProject);

            var expected = new Project
            {
                Id = projectId,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                CreatedAt = createdAt,
                IsActive = false
            };

            SerializerAssert.AreEqual(expected, result);
        }
        #endregion

        #region IMapper<NewProjectRequest, DbProject>
        [Test]
        public void ShouldThrowArgumentNullExceptionWhenNewProjectRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => newProjectRequestMapper.Map(null));
        }

        [Test]
        public void ShouldReturnProjectModelWhenNewProjectRequestIsMapped()
        {
            var result = newProjectRequestMapper.Map(newProjectRequest);

            var expected = new DbProject
            {
                Id = result.Id,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                DepartmentId = departmentId,
                IsActive = true
            };

            SerializerAssert.AreEqual(expected, result);
        }
        #endregion

        #region IMapper<EditProjectRequest, DbProject>
        [Test]
        public void ShouldThrowArgumentNullExceptionWhenEditProjectRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => editProjectRequestMapper.Map(null));
        }

        [Test]
        public void ShouldReturnProjectModelWhenEditProjectRequestIsMapped()
        {
            var result = editProjectRequestMapper.Map(editProjectRequest);

            var expected = new DbProject
            {
                Id = projectId,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                DepartmentId = departmentId,
                IsActive = true
            };

            SerializerAssert.AreEqual(expected, result);
        }
        #endregion
    }
}