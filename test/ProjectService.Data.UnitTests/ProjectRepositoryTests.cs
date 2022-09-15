using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Project;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
  internal class ProjectRepositoryTests
  {
    private IDataProvider _provider;
    private ProjectRepository _repository;
    private DbContextOptions<ProjectServiceDbContext> _dbContext;

    private DbProject _project1;
    private DbProject _project2;
    private List<DbProject> _projects;

    private DbProjectUser _user1;
    private DbProjectUser _user2;
    private DbProjectUser _user3;
    private List<DbProjectUser> _projectsUsers;

    private Guid _creatorId;
    private Guid _userId1;
    private Guid _userId2;
    private Guid _userId3;
    private Guid _projectId1;
    private Guid _projectId2;

    private AutoMocker _mocker;
    private Mock<IHttpContextAccessor> _accessorMock;

    [SetUp]
    public void SetUp()
    {
      CreateProjectsAndUsers();

      CreateMemoryDb();

      SaveProjectsAndUsers();

      _mocker.GetMock<IHttpContextAccessor>().Reset();
    }

    private void CreateProjectsAndUsers()
    {
      _creatorId = Guid.NewGuid();

      _userId1 = Guid.NewGuid();
      _userId2 = Guid.NewGuid();
      _userId3 = Guid.NewGuid();
      _projectId1 = Guid.NewGuid();
      _projectId2 = Guid.NewGuid();

      _user1 = new DbProjectUser
      {
        UserId = _userId1,
        ProjectId = _projectId1,
        IsActive = true,
        Role = (int)ProjectUserRoleType.Observer
      };

      _user2 = new DbProjectUser
      {
        UserId = _userId2,
        ProjectId = _projectId2,
        IsActive = true,
        Role = (int)ProjectUserRoleType.Manager
      };

      _user3 = new DbProjectUser
      {
        UserId = _userId3,
        ProjectId = _projectId2,
        IsActive = false,
        Role = (int)ProjectUserRoleType.Employee
      };

      _projectsUsers = new List<DbProjectUser>
      {
        _user1,
        _user2,
        _user3
      };

      _project1 = new DbProject
      {
        Id = _projectId1,
        Name = "Name1",
        ShortName = "ShortName1"
      };

      _project2 = new DbProject
      {
        Id = _projectId2,
        Name = "Name2",
        ShortName = "ShortName2"
      };

      _projects = new List<DbProject>
      {
        _project1,
        _project2
      };
    }

    public void CreateMemoryDb()
    {
      _mocker = new AutoMocker();
      _dbContext = new DbContextOptionsBuilder<ProjectServiceDbContext>()
        .UseInMemoryDatabase(databaseName: "ProjectServiceTests")
        .Options;

      _accessorMock = new();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _creatorId);

      _accessorMock
        .Setup(x => x.HttpContext.Items)
        .Returns(_items);

      _provider = new ProjectServiceDbContext(_dbContext);
      _repository = new ProjectRepository(_provider, _accessorMock.Object);
    }

    public void SaveProjectsAndUsers()
    {
      _provider.Projects.AddRange(_projects);
      _provider.ProjectsUsers.AddRange(_projectsUsers);
      _provider.Save();
    }

    #region GetUser

    [Test]
    public async Task ShouldReturnProjectByFilterAsync()
    {
      GetProjectFilter filter = new GetProjectFilter
      {
        ProjectId = _projectId1,
        IncludeProjectUsers = true
      };

      SerializerAssert.AreEqual(_project1, await _repository.GetAsync(filter));
    }

    [Test]
    public async Task ShouldReturnProjectByFilterWithoutUsersAsync()
    {
      GetProjectFilter filter = new GetProjectFilter
      {
        ProjectId = _projectId1
      };

      SerializerAssert.AreEqual(_project1, await _repository.GetAsync(filter));
    }

    [Test]
    public void ShouldNotThrowExceptionWhenFilterIsNullAsync()
    {
      Assert.DoesNotThrow(() => _repository.GetAsync((GetProjectFilter)null));
    }

    [Test]
    public async Task ShouldReturnProjectByRequestAsync()
    {
      List<Guid> usersIds = new List<Guid>() { _user1.UserId };
      List<Guid> projectsIds = new List<Guid>() { _projectId1 };

      _mocker
        .Setup<IGetProjectsRequest, List<Guid>>(x => x.UsersIds)
        .Returns(usersIds);

      _mocker
        .Setup<IGetProjectsRequest, List<Guid>>(x => x.ProjectsIds)
        .Returns(projectsIds);

      _mocker
        .Setup<IGetProjectsRequest, bool>(x => x.IncludeUsers)
        .Returns(true);

      SerializerAssert.AreEqual(new List<DbProject>() { _project1 }, await _repository.GetAsync(_mocker.GetMock<IGetProjectsRequest>().Object));
    }

    [Test]
    public async Task ShouldReturnProjectWithUserAsync()
    {
      SerializerAssert.AreEqual(_project1, await _repository.GetProjectWithUserAsync(_projectId1, _userId1));
    }

    #endregion

    #region CreateProject

    [Test]
    public async Task ShouldAddProjectAsync()
    {
      DbProject project = new DbProject
      {
        Id = Guid.NewGuid(),
        Name = "Name",
        ShortName = "ShortName"
      };

      GetProjectFilter filter = new GetProjectFilter
      {
        ProjectId = project.Id
      };

      Assert.DoesNotThrowAsync(async () => await _repository.CreateAsync(project));

      SerializerAssert.AreEqual(project, await _repository.GetAsync(filter));
    }

    [Test]
    public void ShouldReturnNullWhenProjectIsNullAsync()
    {
      Assert.DoesNotThrowAsync(async () => await _repository.CreateAsync(null));
    }

    #endregion

    #region FindProjects

    [Test]
    public async Task ShouldReturnProjectsByFilterWithoutUsersAsync()
    {
      FindProjectsFilter filter = new FindProjectsFilter
      {
        IsAscendingSort = false,
        IncludeDepartment = true,
        UserId = _userId1
      };

      (List<(DbProject dbProject, int usersCount)> dbProjects, int totalCount) expectedResponse =
        (new List<(DbProject dbProject, int usersCount)>
        {
          (_project1, 1),
          (_project2, 2)
        }, 2);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public void ShouldReturnNullWhenFilterIsNull()
    {
      Assert.DoesNotThrowAsync(async () => await _repository.FindAsync(null));
    }

    #endregion

    #region SearchProjects

    [Test]
    public async Task ShouldReturnProjectsWhichContainTextAsync()
    {
      SerializerAssert.AreEqual(new List<DbProject> { _project1 }, await _repository.SearchAsync("1"));
      SerializerAssert.AreEqual(new List<DbProject> { _project1, _project2 }, await _repository.SearchAsync("name"));
    }

    [Test]
    public async Task ShouldReturnNullWhenTextIsNullAsync()
    {
      List<DbProject> expectedResponse = null;
      SerializerAssert.AreEqual(expectedResponse, await _repository.SearchAsync(null));
    }

    #endregion

    #region Edit

    [Test]
    public async Task ShouldEditByProjectId()
    {
      DbProject project = new DbProject
      {
        Id = _projectId1,
        Name = "NewName",
        ShortName = "ShortName1",
        ModifiedBy = _creatorId
      };

      JsonPatchDocument<DbProject> patchPosition;

      patchPosition = new JsonPatchDocument<DbProject>(new List<Operation<DbProject>>
      {
        new Operation<DbProject>(
          "replace",
          $"/{nameof(DbProject.Name)}",
          "",
          $"{project.Name}"),
      }, new CamelCasePropertyNamesContractResolver());

      SerializerAssert.AreEqual(true, await _repository.EditAsync(_projectId1, patchPosition));

      GetProjectFilter filter = new GetProjectFilter
      {
        ProjectId = project.Id
      };

      DbProject editProject = await _repository.GetAsync(filter);

      SerializerAssert.AreEqual(project.Name, editProject.Name);
      SerializerAssert.AreEqual(project.ModifiedBy, editProject.ModifiedBy);
      SerializerAssert.AreEqual(project.ShortName, editProject.ShortName);
      SerializerAssert.AreEqual(project.Id, editProject.Id);
    }

    [Test]
    public async Task ShouldReturnFalseWhenProjectIdIsWrongAsync()
    {
      SerializerAssert.AreEqual(false, await _repository.EditAsync(Guid.NewGuid(), It.IsAny<JsonPatchDocument<DbProject>>()));
    }

    #endregion

    #region DoesExist

    [Test]
    public async Task ShouldReturnIfProjectExistsByIdAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.DoesExistAsync(_projectId1));
      SerializerAssert.AreEqual(false, await _repository.DoesExistAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task ShouldReturnIfProjectExistsByIdsAsync()
    {
      List<Guid> expectedResponse = new List<Guid>
      {
        _projectId1,
        _projectId2
      };

      SerializerAssert.AreEqual(expectedResponse, await _repository.DoExistAsync(new List<Guid> { _projectId1, _projectId2, Guid.NewGuid() }));
    }

    [Test]
    public async Task ShouldReturnIfProjectExistsByNameAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.DoesNameExistAsync(_project1.Name));
      SerializerAssert.AreEqual(false, await _repository.DoesNameExistAsync("alala"));
    }

    [Test]
    public async Task ShouldReturnIfNameExistsAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.DoesNameExistAsync(_project2.Name));
      SerializerAssert.AreEqual(false, await _repository.DoesNameExistAsync("alala"));
    }

    [Test]
    public async Task ShouldReturnIfShortNameExistsAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.DoesShortNameExistAsync(_project2.ShortName));
      SerializerAssert.AreEqual(false, await _repository.DoesShortNameExistAsync("alala"));
    }

    #endregion

    [TearDown]
    public void CleanMemoryDb()
    {
      if (_provider.IsInMemory())
      {
        _provider.EnsureDeleted();
      }
    }
  }
}
