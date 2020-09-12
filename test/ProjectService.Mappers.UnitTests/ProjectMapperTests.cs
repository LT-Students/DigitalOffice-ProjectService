using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.ProjectService.Mappers;
using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db.Entities;
using LT.DigitalOffice.ProjectService.Models.Dto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Mappers
{
    public class ProjectMapperTests
    {
        private IMapper<DbProject, Project> dbToDtoMapper;
        private IMapper<EditProjectRequest, DbProject> editRequestToDbMapper;
        private IMapper<NewProjectRequest, DbProject> newRequestToDbMapper;

        private const string NAME = "Project";
        private const string DESCRIPTION = "DigitalOffice project. The students do the work. Sometimes.";

        private Guid projectId;
        private Guid workerId;
        private Guid departmentId;

        private DbProjectWorkerUser dbWorkersIds;

        private DbProject dbProject;
        private NewProjectRequest newRequest;
        private EditProjectRequest editRequest;

        [SetUp]
        public void SetUp()
        {
            dbToDtoMapper = new ProjectMapper();
            editRequestToDbMapper = new ProjectMapper();
            newRequestToDbMapper = new ProjectMapper();

            projectId = Guid.NewGuid();
            workerId = Guid.NewGuid();
            departmentId = Guid.NewGuid();

            dbWorkersIds = new DbProjectWorkerUser
            {
                ProjectId = projectId,
                Project = dbProject,
                WorkerUserId = workerId
            };

            dbProject = new DbProject
            {
                Id = projectId,
                Name = NAME,
                WorkersUsersIds = new List<DbProjectWorkerUser> { dbWorkersIds }
            };

            newRequest = new NewProjectRequest
            {
                Name = NAME,
                DepartmentId = departmentId,
                Description = DESCRIPTION,
                IsActive = true
            };

            editRequest = new EditProjectRequest
            {
                Name = NAME + "SomeNewText",
                Description = DESCRIPTION + "SomeNewText",
                DepartmentId = Guid.NewGuid(),
                IsActive = false
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => dbToDtoMapper.Map(null));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenNewProjectRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => newRequestToDbMapper.Map(null));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenEditProjectRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => editRequestToDbMapper.Map(null));
        }

        [Test]
        public void ShouldReturnProjectModelWhenDbProjectIsMapped()
        {
            var result = dbToDtoMapper.Map(dbProject);

            var expected = new Project
            {
                Name = dbProject.Name,
                WorkersIds = dbProject.WorkersUsersIds?.Select(x => x.WorkerUserId).ToList()
            };

            SerializerAssert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnDbProjectModelWhenNewProjectRequestIsMapped()
        {
            var newProject = newRequestToDbMapper.Map(newRequest);

            var expectedDbProject = new DbProject
            {
                Id = newProject.Id,
                Name = newRequest.Name,
                DepartmentId = newRequest.DepartmentId,
                Description = newRequest.Description,
                Deferred = false,
                IsActive = newRequest.IsActive
            };

            Assert.IsInstanceOf<Guid>(newProject.Id);
            SerializerAssert.AreEqual(expectedDbProject, newProject);
        }

        [Test]
        public void ShouldReturnDbProjectModelWhenEditProjectRequestIsMapped()
        {
            var editProject = editRequestToDbMapper.Map(editRequest);

            var expectedDbProject = new DbProject
            {
                Name = editRequest.Name,
                DepartmentId = editRequest.DepartmentId,
                Description = editRequest.Description,
                IsActive = editRequest.IsActive,
            };

            Assert.AreEqual(editProject.Id, Guid.Empty);
            SerializerAssert.AreEqual(expectedDbProject, editProject);
        }
    }
}