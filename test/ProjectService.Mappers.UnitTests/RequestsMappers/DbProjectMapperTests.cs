using System;
using System.Collections.Generic;
using LT.DigitalOffice.ProjectService.Mappers.Db;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.ProjectService.Models.Dto.Models.ProjectUser;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Moq;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.UnitTests
{
  internal class DbProjectMapperTests
  {
    private IDbProjectMapper _projectRequestMapper;
    private Mock<IDbEntityImageMapper> _imageMock;

    private CreateProjectRequest _newProject;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _projectRequestMapper = new DbProjectMapper(_imageMock.Object);

      _newProject = new CreateProjectRequest
      {
        Name = "Project for Lanit-Tercom",
        ShortName = "Project",
        Description = "New project for Lanit-Tercom",
        ShortDescription = "Short description",
        DepartmentId = Guid.NewGuid(),
        Status = ProjectStatusType.Active,
        Users = new List<ProjectUserRequest>
          {
            new ProjectUserRequest
            {
              UserId = Guid.NewGuid(),
              Role = ProjectUserRoleType.Manager
            }
          }
      };
    }

    /* [Test]
     public void ShouldThrowArgumentNullExceptionWhenProjectRequestIsNull()
     {
         ProjectRequest projectRequest = null;

         Assert.Throws<ArgumentNullException>(() => _projectRequestMapper.Map(projectRequest, Guid.NewGuid()));
     }

     [Test]
     public void ShouldReturnDbProjectWhenProjectRequestIsMapped()
     {
         var authorId = Guid.NewGuid();
         List<Guid> users = new();
         var dbProject = _projectRequestMapper.Map(_newProject, authorId,  users);

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
                     UserId = _newProject.Users.ElementAt(0).UserId,
                     IsActive = true
                 }
             }
         };

         SerializerAssert.AreEqual(expectedDbProject, dbProject);
     }*/
  }
}
