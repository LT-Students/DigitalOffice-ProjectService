using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.User;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Data.UnitTests
{
  internal class ProjectUserRepositoryTests
  {
    private IDataProvider _provider;
    private ProjectUserRepository _userRepository;
    private DbContextOptions<ProjectServiceDbContext> _dbContext;

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
      CreateUsers();

      CreateMemoryDb();

      SaveUsers();

      _mocker.GetMock<IHttpContextAccessor>().Reset();
    }

    private void CreateUsers()
    {
      _creatorId = Guid.NewGuid();

      _userId1 = Guid.NewGuid();
      _userId2 = Guid.NewGuid();
      _userId3 = Guid.NewGuid();
      _projectId1 = Guid.NewGuid();
      _projectId2 = Guid.NewGuid();

      _user1 = new DbProjectUser
      {
        Id = Guid.NewGuid(),
        UserId = _userId1,
        ProjectId = _projectId1,
        IsActive = true,
        Role = (int)ProjectUserRoleType.Observer
      };

      _user2 = new DbProjectUser
      {
        Id = Guid.NewGuid(),
        UserId = _userId2,
        ProjectId = _projectId2,
        IsActive = true,
        Role = (int)ProjectUserRoleType.Manager
      };

      _user3 = new DbProjectUser
      {
        Id = Guid.NewGuid(),
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
      _userRepository = new ProjectUserRepository(_provider, _accessorMock.Object);
    }

    public void SaveUsers()
    {
      _provider.ProjectsUsers.AddRange(_projectsUsers);
      _provider.Save();
    }

    #region GetUser

    [Test]
    public async Task ShouldReturnUserForIdsAsync()
    {
      SerializerAssert.AreEqual(new List<DbProjectUser> { _user1 }, await _userRepository.GetAsync(new List<Guid> { _userId1 }));
    }

    [Test]
    public async Task ShouldReturnNullUserForIdsAsync()
    {
      SerializerAssert.AreEqual(new List<DbProjectUser>(), await _userRepository.GetAsync(It.IsAny<List<Guid>>()));
    }

    [Test]
    public async Task ShouldReturnUserForRequestAsync()
    {
      List<Guid> usersIds = new List<Guid>() { _user1.UserId };
      List<Guid> projectsIds = new List<Guid>() { _projectId1 };

      _mocker
        .Setup<IGetProjectsUsersRequest, List<Guid>>(x => x.UsersIds)
        .Returns(usersIds);

      _mocker
        .Setup<IGetProjectsUsersRequest, List<Guid>>(x => x.ProjectsIds)
        .Returns(projectsIds);

      (List<DbProjectUser>, int totalCount) expectedResponse = (new List<DbProjectUser>() { _user1 }, 1);

      SerializerAssert.AreEqual(expectedResponse, await _userRepository.GetAsync(_mocker.GetMock<IGetProjectsUsersRequest>().Object));
    }

    [Test]
    public async Task ShouldReturnAllUsersForRequestAsync()
    {
      (List<DbProjectUser>, int totalCount) expectedResponse = (new List<DbProjectUser>() { _user1, _user2 }, 2);

      SerializerAssert.AreEqual(expectedResponse, await _userRepository.GetAsync(_mocker.GetMock<IGetProjectsUsersRequest>().Object));
    }

    [Test]
    public async Task ShouldReturnAllUsersFromProjectAsync()
    {
      SerializerAssert.AreEqual(new List<DbProjectUser> { _user2, _user3 }, await _userRepository.GetAsync(_projectId2, default));
    }

    [Test]
    public async Task ShouldReturnAllActiveUsersFromProjectAsync()
    {
      SerializerAssert.AreEqual(new List<DbProjectUser> { _user2 }, await _userRepository.GetAsync(_projectId2, true));
    }

    #endregion

    #region GetExistingUsers

    [Test]
    public async Task ShouldReturnUserForIdsAndProjectAsync()
    {
      SerializerAssert.AreEqual(new List<DbProjectUser> { _user2 }, await _userRepository.GetExistingUsersAsync(_projectId2, new List<Guid> { _userId2 }));
    }

    #endregion

    #region GetUserRole

    [Test]
    public async Task ShouldReturnUserRoleAsync()
    {
      SerializerAssert.AreEqual(ProjectUserRoleType.Manager, await _userRepository.GetUserRoleAsync(_projectId2, _userId2));
    }

    #endregion

    #region AddUser

    [Test]
    public async Task ShouldAddUserAsync()
    {
      DbProjectUser user = new DbProjectUser
      {
        UserId = Guid.NewGuid(),
        ProjectId = _projectId1,
        IsActive = true,
        Role = (int)ProjectUserRoleType.Employee
      };

      await _userRepository.CreateAsync(new List<DbProjectUser> { user });

      SerializerAssert.AreEqual(new List<DbProjectUser> { user }, await _userRepository.GetAsync(new List<Guid> { user.UserId }));
    }

    [Test]
    public void ShouldNotThrowExceptionForAddNullUserAsync()
    {
      Assert.DoesNotThrowAsync(() => _userRepository.CreateAsync(null));
    }

    #endregion

    #region EditIsActive

    [Test]
    public async Task ShouldEditIsActiveAsync()
    {
      SerializerAssert.AreEqual(true, await _userRepository.EditIsActiveAsync(new List<DbProjectUser> { _user3 }, Guid.NewGuid()));
      SerializerAssert.AreEqual(new List<DbProjectUser> { _user3 }, await _userRepository.GetAsync(new List<Guid> { _userId3 }));
    }

    [Test]
    public async Task ShouldNotEditIsActiveAsync()
    {
      SerializerAssert.AreEqual(false, await _userRepository.EditIsActiveAsync(null, Guid.NewGuid()));
    }

    #endregion

    #region DoExistAsync

    [Test]
    public async Task ShouldAnswerForUserWithAllRoles()
    {
      SerializerAssert.AreEqual(true, await _userRepository.DoesExistAsync(_userId1, _projectId1));
      SerializerAssert.AreEqual(false, await _userRepository.DoesExistAsync(_userId2, _projectId1));
    }

    [Test]
    public async Task ShouldAnswerForUserWithManagerRole()
    {
      SerializerAssert.AreEqual(true, await _userRepository.DoesExistAsync(_userId2, _projectId2, true));
      SerializerAssert.AreEqual(false, await _userRepository.DoesExistAsync(_userId1, _projectId1, true));
    }

    [Test]
    public async Task ShouldAnswerForUsersWithAllRoles()
    {
      SerializerAssert.AreEqual(new List<Guid> { _userId2, _userId3 }, await _userRepository.DoExistAsync(_projectId2, new List<Guid> { _userId2, _userId3 }));
      SerializerAssert.AreEqual(new List<Guid>(), await _userRepository.DoExistAsync(_projectId1, new List<Guid> { _userId2 }));
    }

    [Test]
    public async Task ShouldAnswerForUsersWithManagerRole()
    {
      SerializerAssert.AreEqual(new List<Guid> { _userId2 }, await _userRepository.DoExistAsync(_projectId2, new List<Guid> { _userId2, _userId3 }, true));
      SerializerAssert.AreEqual(null, await _userRepository.DoExistAsync(_projectId2, null, true));
    }

    #endregion

    #region RemoveAsync

    [Test]
    public async Task ShouldRemoveByUserId()
    {
      SerializerAssert.AreEqual(new List<Guid> { _projectId1 }, await _userRepository.RemoveAsync(_userId1, Guid.NewGuid()));
      SerializerAssert.AreEqual(new List<DbProjectUser>(), await _userRepository.GetAsync(new List<Guid> { _userId1 }));
      SerializerAssert.AreEqual(new List<DbProjectUser> { _user1 }, await _userRepository.GetExistingUsersAsync(_projectId1, new List<Guid> { _userId1 }));
    }

    [Test]
    public async Task ShouldNotRemoveByUserId()
    {
      SerializerAssert.AreEqual(new List<Guid> { }, await _userRepository.RemoveAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Test]
    public async Task ShouldNotRemoveByUsersId()
    {
      SerializerAssert.AreEqual(true, await _userRepository.RemoveAsync(_projectId1, new List<Guid>() { _userId1 }));
      SerializerAssert.AreEqual(new List<DbProjectUser>(), await _userRepository.GetAsync(new List<Guid> { _userId1 }));
      SerializerAssert.AreEqual(new List<DbProjectUser> { _user1 }, await _userRepository.GetExistingUsersAsync(_projectId1, new List<Guid> { _userId1 }));

      SerializerAssert.AreEqual(false, await _userRepository.RemoveAsync(Guid.NewGuid(), (IEnumerable<Guid>)default));
    }

    #endregion

    #region EditAsync

    [Test]
    public async Task ShouldEditByUserId()
    {
      DbProjectUser user = new DbProjectUser
      {
        Id = _user1.Id,
        UserId = _userId1,
        ProjectId = _projectId1,
        CreatedBy = _creatorId,
        IsActive = true,
        Role = (int)ProjectUserRoleType.Manager
      };

      EditProjectUsersRoleRequest request = new EditProjectUsersRoleRequest
      {
        Role = ProjectUserRoleType.Manager,
        UsersIds = new List<Guid>
        {
          _userId1
        }
      };

      SerializerAssert.AreEqual(true, await _userRepository.EditAsync(_projectId1, request));

      SerializerAssert.AreEqual(new List<DbProjectUser> { user }, await _userRepository.GetAsync(new List<Guid> { _userId1 }));
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
