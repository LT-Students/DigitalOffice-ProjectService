using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    class FindTasksCommandTests
    {
        private IEnumerable<DbTask> _dbTasks;
        private IEnumerable<TaskInfo> _taskInfo;
        private IEnumerable<DbProjectUser> _dbProjectUsers;

        private AutoMocker _mocker;
        private IFindTasksCommand _command;
        IDictionary<object, object> _contextValues;

        #region Setup
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _taskInfo = new List<TaskInfo>();
            _dbTasks = new List<DbTask>();

            _contextValues = new Dictionary<object, object>();
            _contextValues.Add("UserId", Guid.NewGuid());

            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<FindTasksCommand>();

            for (int i = 0; i <= 3; i++)
            {
                _dbTasks = _dbTasks.Append(
                    new DbTask
                    {
                        Id = Guid.NewGuid(),
                        Name = "Create Smth",
                        Description = "Create smth in somewhere",
                        PlannedMinutes = 30,
                        AssignedTo = Guid.NewGuid(),
                        AuthorId = Guid.NewGuid(),
                        ProjectId = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        ParentId = Guid.NewGuid(),
                        Number = 2,
                        PriorityId = Guid.NewGuid(),
                        StatusId = Guid.NewGuid(),
                        TypeId = Guid.NewGuid()
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

            foreach (var dbTask in _dbTasks)
            {
                if (dbTask.ProjectId != _dbProjectUsers.ElementAt(0).ProjectId)
                {
                    continue;
                }

                _taskInfo = _taskInfo.Append(
                    new TaskInfo
                    {
                        Id = dbTask.Id,
                        /*AssignedTo = dbTask.AssignedTo,
                        AuthorId = dbTask.AuthorId,
                        CreatedAt = dbTask.CreatedAt,
                        Description = dbTask.Description,
                        Name = dbTask.Name,
                        Number = dbTask.Number,
                        ParentId = dbTask.ParentId,
                        PlannedMinutes = dbTask.PlannedMinutes,
                        PriorityId = dbTask.PriorityId,
                        ProjectId = dbTask.ProjectId,
                        StatusId = dbTask.StatusId,
                        TypeId = dbTask.TypeId*/
                    });
            }
        }
        #endregion

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<ITaskInfoMapper>().Reset();
            _mocker.GetMock<ITaskRepository>().Reset();
        }

        [Test]
        public void ShouldThrowExceptionWhenFilterIsNull()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _taskInfo.Count(); ;

            FindTasksFilter filter = null;

            Assert.Throws<ArgumentNullException>(() => _command.Execute(filter, skipCount, takeCount));
            //_mocker.Verify<ITaskInfoMapper, TaskInfo>(x => x.Map(It.IsAny<DbTask>()), Times.Never);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<List<Guid>>(), skipCount, takeCount, out totalCount), Times.Never);
        }

        [Test]
        public void ShouldReturnFindResponseSuccesfull()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _taskInfo.Count();

            var result = new FindResponse<TaskInfo>
            {
                TotalCount = _taskInfo.Count(),
                Body = _taskInfo,
            };

            //var projectIds = _taskInfo.Select(x => x.ProjectId);
            //var dbTasks = _dbTasks.Where(x => projectIds.Contains(x.ProjectId));

            var filter = new FindTasksFilter();
            filter.Assign = _dbTasks.ElementAt(0).AssignedTo;

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_contextValues);

            _mocker
                .Setup<IUserRepository, IEnumerable<DbProjectUser>>(x => x.Find((Guid)_contextValues["UserId"]))
                .Returns(_dbProjectUsers);

            /*_mocker
                .Setup<ITaskRepository, IEnumerable<DbTask>>(x => x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount))
                .Returns(dbTasks);*/

            /*_mocker
                .SetupSequence<ITaskInfoMapper, TaskInfo>(x => x.Map(It.IsAny<DbTask>()))
                .Returns(_taskInfo.ElementAt(0));*/

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
           // _mocker.Verify<ITaskInfoMapper, TaskInfo>(x => x.Map(It.IsAny<DbTask>()), Times.Exactly(_taskInfo.Count()));
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, It.IsAny<IEnumerable<Guid>>(), skipCount, takeCount, out totalCount), Times.Once);
        }
    }
}
