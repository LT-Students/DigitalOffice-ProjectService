using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.UnitTests
{
    internal class DbProjectMapperTests
    {
        private IDbProjectMapper _projectRequestMapper;

        private ProjectRequest _newProject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _projectRequestMapper = new DbProjectMapper();

            _newProject = new ProjectRequest
            {
                Name = "Project for Lanit-Tercom",
                ShortName = "Project",
                Description = "New project for Lanit-Tercom",
                ShortDescription = "Short description",
                DepartmentId = Guid.NewGuid(),
                Status = ProjectStatusType.Abandoned,
                Users = new List<ProjectUser>
                {
                    new ProjectUser
                    {
                        Id = Guid.NewGuid(),
                        Role = UserRoleType.Admin
                    }
                }
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenProjectRequestIsNull()
        {
            ProjectRequest projectRequest = null;

            Assert.Throws<ArgumentNullException>(() => _projectRequestMapper.Map(projectRequest, Guid.NewGuid()));
        }

        [Test]
        public void ShouldReturnDbProjectWhenProjectRequestIsMapped()
        {
            var authorId = Guid.NewGuid();

            var dbProject = _projectRequestMapper.Map(_newProject, authorId);

            var expectedDbProject = new DbProject
            {
                Id = dbProject.Id,
                AuthorId = authorId,
                ShortName = _newProject.ShortName,
                DepartmentId = _newProject.DepartmentId,
                Name = _newProject.Name,
                Description = _newProject.Description,
                CreatedAt = dbProject.CreatedAt,
                ShortDescription = _newProject.ShortDescription,
                Status = (int)_newProject.Status,
                Users = new List<DbProjectUser>
                {
                    new DbProjectUser
                    {
                        Id = dbProject.Users.ElementAt(0).Id,
                        Role = (int)_newProject.Users.ElementAt(0).Role,
                        AddedOn = dbProject.Users.ElementAt(0).AddedOn,
                        ProjectId = dbProject.Id,
                        UserId = _newProject.Users.ElementAt(0).Id,
                        IsActive = true
                    }
                }
            };

            SerializerAssert.AreEqual(expectedDbProject, dbProject);
        }
    }
}
