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
        private Mock<Response<IOperationResult<IGetUserDataResponse>>> _responseAuthorFirst;
        private Mock<Response<IOperationResult<IGetUserDataResponse>>> _responseAnssignedUserFirst;
        private Mock<Response<IOperationResult<IGetUserDataResponse>>> _responseAuthorSecond;
        private Mock<Response<IOperationResult<IGetUserDataResponse>>> _responseAnssignedUserSecond;

        #region Setup

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbTasks = new List<DbTask>();

            _contextValues = new Dictionary<object, object>();
            _contextValues.Add("UserId", Guid.NewGuid());

            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<FindTasksCommand>();

            var assignedUserData = new Mock<IGetUserDataResponse>();
            var authorData = new Mock<IGetUserDataResponse>();

            _responseAuthorFirst = new Mock<Response<IOperationResult<IGetUserDataResponse>>>();
            _responseAnssignedUserFirst = new Mock<Response<IOperationResult<IGetUserDataResponse>>>();
            _responseAuthorSecond = new Mock<Response<IOperationResult<IGetUserDataResponse>>>();
            _responseAnssignedUserSecond = new Mock<Response<IOperationResult<IGetUserDataResponse>>>();

            assignedUserData.Setup(x => x.FirstName).Returns("Ivan");
            assignedUserData.Setup(x => x.LastName).Returns("Ivanov");

            authorData.Setup(x => x.FirstName).Returns("Semen");
            authorData.Setup(x => x.LastName).Returns("Semenov");

            _responseAuthorFirst.Setup(x => x.Message.Body).Returns(authorData.Object);
            _responseAnssignedUserFirst.Setup(x => x.Message.Body).Returns(assignedUserData.Object);
            _responseAuthorSecond.Setup(x => x.Message.Body).Returns(authorData.Object);
            _responseAnssignedUserSecond.Setup(x => x.Message.Body).Returns(assignedUserData.Object);

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
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<ITaskInfoMapper>().Reset();
            _mocker.GetMock<ITaskRepository>().Reset();
            _mocker.GetMock<IRequestClient<IGetUserDataRequest>>().Reset();
            _mocker.GetMock<IHttpContextAccessor>().Reset();
            _mocker.GetMock<IAccessValidator>().Reset();

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
                x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()), Times.Never);
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
            filter.Assign = _dbTasks.ElementAt(0).AssignedTo;

            SerializerAssert.AreEqual(expectedResult, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()), Times.Never);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<List<Guid>>(), skipCount, takeCount, out totalCount), Times.Never);
            _mocker.Verify<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default), Times.Never);
        }

        [Test]
        public void ShouldReturnFindResponseWhenBrokerResponseAssignedUserIsNotSucces()
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
            filter.Assign = _dbTasks.ElementAt(0).AssignedTo;

            _responseAuthorFirst.Setup(x => x.Message.IsSuccess).Returns(true);
            _responseAnssignedUserFirst.Setup(x => x.Message.IsSuccess).Returns(false);

            _responseAnssignedUserFirst.Setup(x => x.Message.Errors).Returns(new List<string>());

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
                    x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()))
                .Returns(_tasksInfo.ElementAt(0));

            _mocker
                .SetupSequence<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseAnssignedUserFirst.Object))
                .Returns(Task.FromResult(_responseAuthorFirst.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Never);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()), Times.Exactly(_tasksInfo.Count()));
            _mocker.Verify<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default), Times.Exactly(_tasksInfo.Count() * 2));
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
            filter.Assign = _dbTasks.ElementAt(0).AssignedTo;

            _responseAuthorFirst.Setup(x => x.Message.IsSuccess).Returns(true);
            _responseAnssignedUserFirst.Setup(x => x.Message.IsSuccess).Returns(true);
            _responseAuthorSecond.Setup(x => x.Message.IsSuccess).Returns(true);
            _responseAnssignedUserSecond.Setup(x => x.Message.IsSuccess).Returns(true);

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
                    x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()))
                .Returns(_fullTasksInfo.ElementAt(0))
                .Returns(_fullTasksInfo.ElementAt(1));

            _mocker
                .SetupSequence<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseAnssignedUserFirst.Object))
                .Returns(Task.FromResult(_responseAuthorFirst.Object))
                .Returns(Task.FromResult(_responseAnssignedUserSecond.Object))
                .Returns(Task.FromResult(_responseAuthorSecond.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Once);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()), Times.Exactly(_fullTasksInfo.Count()));
            _mocker.Verify<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default), Times.Exactly(_fullTasksInfo.Count() * 2));
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
            filter.Assign = _dbTasks.ElementAt(0).AssignedTo;
            _responseAuthorFirst.Setup(x => x.Message.IsSuccess).Returns(false);
            _responseAnssignedUserFirst.Setup(x => x.Message.IsSuccess).Returns(true);

            _responseAuthorFirst.Setup(x => x.Message.Errors).Returns(new List<string>());

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
                    x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()))
                .Returns(_tasksInfo.ElementAt(0));

            _mocker
                .SetupSequence<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseAnssignedUserFirst.Object))
                .Returns(Task.FromResult(_responseAuthorFirst.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Never);
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()), Times.Exactly(_tasksInfo.Count()));
            _mocker.Verify<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default), Times.Exactly(_tasksInfo.Count() * 2));
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
            filter.Assign = _dbTasks.ElementAt(0).AssignedTo;
            _responseAuthorFirst.Setup(x => x.Message.IsSuccess).Returns(true);
            _responseAnssignedUserFirst.Setup(x => x.Message.IsSuccess).Returns(true);

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
                    x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()))
                .Returns(_tasksInfo.ElementAt(0));

            var author = IGetUserDataRequest.CreateObj(It.IsAny<Guid>());
            var user = IGetUserDataRequest.CreateObj(It.IsAny<Guid>());

            _mocker
                .SetupSequence<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(_responseAnssignedUserFirst.Object))
                .Returns(Task.FromResult(_responseAuthorFirst.Object));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items, Times.Exactly(2));
            _mocker.Verify<IAccessValidator, bool>(x => x.IsAdmin(null), Times.Never);
            _mocker.Verify<IRequestClient<IGetUserDataRequest>, Task<Response<IOperationResult<IGetUserDataResponse>>>>(x =>
                    x.GetResponse<IOperationResult<IGetUserDataResponse>>(It.IsAny<object>(), default, default), Times.Exactly(_tasksInfo.Count() * 2));
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x =>
                x.Map(It.IsAny<DbTask>(), It.IsAny<IGetUserDataResponse>(), It.IsAny<IGetUserDataResponse>()), Times.Exactly(_tasksInfo.Count()));
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount), Times.Once);
        }
    }
}
