using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    class FindTasksCommandTests
    {
        private IEnumerable<DbTask> _dbTasks;
        private IEnumerable<TaskInfo> _tasksInfo;
        private IEnumerable<TaskInfo> _fullTasksInfo;
        private IEnumerable<DbProjectUser> _dbProjectUsers;

        private AutoMocker _mocker;
        private IFindTasksCommand _command;
        private IDictionary<object, object> _contextValues;
        private Mock<Response<IOperationResult<IGetUsersDataResponse>>> _responseUsersData;

        #region Setup

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbTasks = new List<DbTask>();

            _contextValues = new Dictionary<object, object>();
            _contextValues.Add("UserId", Guid.NewGuid());

            for (int i = 0; i <= 1; i++)
            {
                var projectId = Guid.NewGuid();

                _dbTasks = _dbTasks.Append(
                    new DbTask
                    {
                        Id = Guid.NewGuid(),
                        Name = "Create Smth",
                        Description = "Create smth in somewhere",
                        PlannedMinutes = 30,
                        AssignedTo = Guid.NewGuid(),
                        AuthorId = Guid.NewGuid(),
                        ProjectId = projectId,
                        CreatedAt = DateTime.UtcNow,
                        ParentId = Guid.NewGuid(),
                        Number = 2,
                        StatusId = Guid.NewGuid(),
                        TypeId = Guid.NewGuid(),
                        Type = new DbTaskProperty
                        {
                            Id = Guid.NewGuid(),
                            Name = "Feature"
                        },
                        Status = new DbTaskProperty
                        {
                            Id = Guid.NewGuid(),
                            Name = "New"
                        },
                        Priority = new DbTaskProperty
                        {
                            Id = Guid.NewGuid(),
                            Name = "First"
                        },
                        Project = new DbProject
                        {
                            Id = projectId,
                            ShortName = "DO"
                        }
                    });
            }

            _dbProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _dbTasks.ElementAt(0).ProjectId,
                    UserId = (Guid)_contextValues["UserId"],
                    AddedOn = DateTime.Now,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _dbTasks.ElementAt(0).ProjectId,
                    UserId = (Guid)_contextValues["UserId"],
                    AddedOn = DateTime.Now,
                    IsActive = true
                }
            };

            _fullTasksInfo = new List<TaskInfo>();
            foreach (var dbTask in _dbTasks)
            {
                _fullTasksInfo = _fullTasksInfo.Append(
                    new TaskInfo
                    {
                        Id = dbTask.Id,
                        Name = "Create Smth",
                        Description = "Create smth in somewhere",
                        PlannedMinutes = 30,
                        AssignedTo = new UserTaskInfo
                        {
                            Id = dbTask.AssignedTo,
                            FirstName = "Ivan",
                            LastName = "Ivanov"
                        },
                        Author = new UserTaskInfo
                        {
                            Id = dbTask.AuthorId,
                            FirstName = "Semen",
                            LastName = "Semenov"
                        },
                        Project = new ProjectTaskInfo
                        {
                            Id = dbTask.ProjectId,
                            ShortName = "DO"
                        },
                        CreatedAt = dbTask.CreatedAt,
                        Number = 2,
                        PriorityName = "First",
                        StatusName = "New",
                        TypeName = "Feature"
                    });
            }

            var users = new List<UserData>();
            foreach (var dbTask in _dbTasks)
            {
                users.Add(new UserData
                {
                    Id = dbTask.AssignedTo.Value,
                    FirstName = "Ivan",
                    LastName = "Ivanov"
                });

                users.Add(new UserData
                {
                    Id = dbTask.AuthorId,
                    FirstName = "Semen",
                    LastName = "Semenov"
                });
            }


            var getUsersDataResponse = new Mock<IGetUsersDataResponse>();
            _responseUsersData = new Mock<Response<IOperationResult<IGetUsersDataResponse>>>();

            getUsersDataResponse.Setup(x => x.UsersData).Returns(users);

            _responseUsersData.Setup(x => x.Message.Body).Returns(getUsersDataResponse.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<FindTasksCommand>();

            _responseUsersData.Reset();

            _tasksInfo = new List<TaskInfo>();
            foreach (var dbTask in _dbTasks)
            {
                if (dbTask.ProjectId != _dbProjectUsers.ElementAt(0).ProjectId)
                {
                    continue;
                }

                _tasksInfo = _tasksInfo.Append(
                    new TaskInfo
                    {
                        Id = dbTask.Id,
                        Name = "Create Smth",
                        Description = "Create smth in somewhere",
                        PlannedMinutes = 30,
                        AssignedTo = new UserTaskInfo
                        {
                            Id = dbTask.AssignedTo,
                            FirstName = "Ivan",
                            LastName = "Ivanov"
                        },
                        Author = new UserTaskInfo
                        {
                            Id = dbTask.AuthorId,
                            FirstName = "Semen",
                            LastName = "Semenov"
                        },
                        Project = new ProjectTaskInfo
                        {
                            Id = dbTask.ProjectId,
                            ShortName = "DO"
                        },
                        CreatedAt = dbTask.CreatedAt,
                        Number = 2,
                        PriorityName = "First",
                        StatusName = "New",
                        TypeName = "Feature"
                    });
            }
        }

        #endregion

        [Test]
        public void ShouldThrowExceptionWhenFilterIsNull()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _tasksInfo.Count(); ;

            FindTasksFilter filter = null;

            var arg = It.IsAny<IGetUserDataResponse>();
            Assert.Throws<ArgumentNullException>(() => _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()), Times.Never);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<List<Guid>>(), skipCount, takeCount, out totalCount), Times.Never);
            _mocker.Verify<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default), Times.Never);
        }

        [Test]
        public void ShouldReturnEmptyFindResponseWhenUserHasNotRights()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _tasksInfo.Count();

            var expectedResult = new FindResponse<TaskInfo>();

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_contextValues);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(false);

            var filter = new FindTasksFilter();
            filter.AssignTo = _dbTasks.ElementAt(0).AssignedTo;

            SerializerAssert.AreEqual(expectedResult, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()), Times.Never);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<List<Guid>>(), skipCount, takeCount, out totalCount), Times.Never);
            _mocker.Verify<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default), Times.Never);
        }

        [Test]
        public void ShouldReturnFindResponseWhenBrokerResponseIsNotSucces()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _tasksInfo.Count();

            var result = new FindResponse<TaskInfo>
            {
                TotalCount = _tasksInfo.Count(),
                Body = _tasksInfo,
            };

            var projectIds = _tasksInfo.Select(x => x.Project.Id);
            var dbTasks = _dbTasks.Where(x => projectIds.Contains(x.ProjectId));

            var filter = new FindTasksFilter();
            filter.AssignTo = _dbTasks.ElementAt(0).AssignedTo;

            _responseUsersData.Setup(x => x.Message.IsSuccess).Returns(true);

            _responseUsersData.Setup(x => x.Message.Errors).Returns(new List<string>());

            foreach (var task in _tasksInfo)
            {
                task.AssignedTo.FirstName = null;
                task.AssignedTo.LastName = null;
            }

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_contextValues);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(false);

            _mocker
                .Setup<IUserRepository, IEnumerable<DbProjectUser>>(x => x.Find((Guid)_contextValues["UserId"]))
                .Returns(_dbProjectUsers);

            _mocker
                .Setup<ITaskRepository, IEnumerable<DbTask>>(x => x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount))
                .Returns(dbTasks);

            _mocker
                .SetupSequence<ITaskInfoMapper, TaskInfo>(x =>
                    x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()))
                .Returns(_tasksInfo.ElementAt(0));

            _mocker
                .Setup<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseUsersData.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Never);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()), Times.Exactly(_tasksInfo.Count()));
            _mocker.Verify<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default), Times.Once);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount), Times.Once);
        }

        [Test]
        public void ShouldReturnFullFindResponseWhenUserIsAdmin()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _fullTasksInfo.Count();

            var result = new FindResponse<TaskInfo>
            {
                TotalCount = _fullTasksInfo.Count(),
                Body = _fullTasksInfo,
            };

            var filter = new FindTasksFilter();
            filter.AssignTo = _dbTasks.ElementAt(0).AssignedTo;

            _responseUsersData.Setup(x => x.Message.IsSuccess).Returns(true);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_contextValues);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            _mocker
                .Setup<IUserRepository, IEnumerable<DbProjectUser>>(x => x.Find((Guid)_contextValues["UserId"]))
                .Returns(new List<DbProjectUser>());

            _mocker
                .Setup<ITaskRepository, IEnumerable<DbTask>>(x => x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount))
                .Returns(_dbTasks);

            _mocker
                .SetupSequence<ITaskInfoMapper, TaskInfo>(x =>
                    x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()))
                .Returns(_fullTasksInfo.ElementAt(0))
                .Returns(_fullTasksInfo.ElementAt(1));

            _mocker
                .Setup<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseUsersData.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()), Times.Exactly(_fullTasksInfo.Count()));
            _mocker.Verify<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default), Times.Once);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount), Times.Once);
        }

        [Test]
        public void ShouldReturnFindResponseWhenBrokerResponseAuthorIsNotSucces()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _tasksInfo.Count();

            var result = new FindResponse<TaskInfo>
            {
                TotalCount = _tasksInfo.Count(),
                Body = _tasksInfo,
            };

            var projectIds = _tasksInfo.Select(x => x.Project.Id);
            var dbTasks = _dbTasks.Where(x => projectIds.Contains(x.ProjectId));

            var filter = new FindTasksFilter();
            filter.AssignTo = _dbTasks.ElementAt(0).AssignedTo;
            _responseUsersData.Setup(x => x.Message.IsSuccess).Returns(false);

            _responseUsersData.Setup(x => x.Message.Errors).Returns(new List<string>());

            foreach (var task in _tasksInfo)
            {
                task.Author = null;
            }

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_contextValues);

            _mocker
                .Setup<IUserRepository, IEnumerable<DbProjectUser>>(x => x.Find((Guid)_contextValues["UserId"]))
                .Returns(_dbProjectUsers);

            _mocker
                .Setup<ITaskRepository, IEnumerable<DbTask>>(x => x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount))
                .Returns(dbTasks);

            _mocker
                .SetupSequence<ITaskInfoMapper, TaskInfo>(x =>
                    x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()))
                .Returns(_tasksInfo.ElementAt(0));

            _mocker
                .SetupSequence<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseUsersData.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Never);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()), Times.Exactly(_tasksInfo.Count()));
            _mocker.Verify<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default), Times.Once);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount), Times.Once);
        }

        [Test]
        public void ShouldReturnFindResponseSuccesfull()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _tasksInfo.Count();

            var result = new FindResponse<TaskInfo>
            {
                TotalCount = _tasksInfo.Count(),
                Body = _tasksInfo,
            };

            var projectIds = _tasksInfo.Select(x => x.Project.Id);
            var dbTasks = _dbTasks.Where(x => projectIds.Contains(x.ProjectId));

            var filter = new FindTasksFilter();
            filter.AssignTo = _dbTasks.ElementAt(0).AssignedTo;
            _responseUsersData.Setup(x => x.Message.IsSuccess).Returns(true);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_contextValues);

            _mocker
                .Setup<IUserRepository, IEnumerable<DbProjectUser>>(x => x.Find((Guid)_contextValues["UserId"]))
                .Returns(_dbProjectUsers);

            _mocker
                .Setup<ITaskRepository, IEnumerable<DbTask>>(x => x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount))
                .Returns(dbTasks);

            _mocker
                .SetupSequence<ITaskInfoMapper, TaskInfo>(x =>
                    x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()))
                .Returns(_tasksInfo.ElementAt(0));

            _mocker
                .SetupSequence<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseUsersData.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Never);
            _mocker.Verify<IRequestClient<IGetUsersDataRequest>, Task<Response<IOperationResult<IGetUsersDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUsersDataResponse>>(It.IsAny<object>(), default, default), Times.Once);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<UserData>(), It.IsAny<UserData>()), Times.Exactly(_tasksInfo.Count()));
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount), Times.Once);
        }
    }
}
