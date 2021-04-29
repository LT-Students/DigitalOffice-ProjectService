using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using LT.DigitalOffice.UnitTestKernel;
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

        private IFindTasksCommand _command;
        private AutoMocker _mocker;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _taskInfo = new List<TaskInfo>();
            _dbTasks = new List<DbTask>();

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
                        Deadline = DateTime.UtcNow,
                        ProjectId = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        ParentId = Guid.NewGuid(),
                        Number = 2,
                        PriorityId = Guid.NewGuid(),
                        StatusId = Guid.NewGuid(),
                        TypeId = Guid.NewGuid()
                    });
            }

            foreach (var dbTask in _dbTasks)
            {
                _taskInfo = _taskInfo.Append(
                    new TaskInfo
                    {
                        Id = dbTask.Id,
                        AssignedTo = dbTask.AssignedTo,
                        AuthorId = dbTask.AuthorId,
                        CreatedAt = dbTask.CreatedAt,
                        Deadline = dbTask.Deadline,
                        Description = dbTask.Description,
                        Name = dbTask.Name,
                        Number = dbTask.Number,
                        ParentId = dbTask.ParentId,
                        PlannedMinutes = dbTask.PlannedMinutes,
                        PriorityId = dbTask.PriorityId,
                        ProjectId = dbTask.ProjectId,
                        StatusId = dbTask.StatusId,
                        TypeId= dbTask.TypeId
                    });
            }
        }

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
            int totalCount = _dbTasks.Count();

            FindTasksFilter filter = null;

            Assert.Throws<ArgumentNullException>(() => _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x => x.Map(It.IsAny<DbTask>()), Times.Never);
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, skipCount, takeCount, out totalCount), Times.Never);
        }

        [Test]
        public void ShouldReturnFindResponseSuccesfull()
        {
            int skipCount = 0;
            int takeCount = _dbTasks.Count();
            int totalCount = _dbTasks.Count();

            var result = new FindResponse<TaskInfo>
            {
                TotalCount = _dbTasks.Count(),
                Body = _taskInfo,
            };

            var filter = new FindTasksFilter();
            filter.Assign = _dbTasks.ElementAt(0).AssignedTo;

            _mocker
                .Setup<ITaskRepository, IEnumerable<DbTask>>(x => x.Find(filter, skipCount, takeCount, out totalCount))
                .Returns(_dbTasks);

            _mocker
                .SetupSequence<ITaskInfoMapper, TaskInfo>(x => x.Map(It.IsAny<DbTask>()))
                .Returns(_taskInfo.ElementAt(0))
                .Returns(_taskInfo.ElementAt(1))
                .Returns(_taskInfo.ElementAt(2))
                .Returns(_taskInfo.ElementAt(3));

            SerializerAssert.AreEqual(result, _command.Execute(filter, skipCount, takeCount));
            _mocker.Verify<ITaskInfoMapper, TaskInfo>(x => x.Map(It.IsAny<DbTask>()), Times.Exactly(_dbTasks.Count()));
            _mocker.Verify<ITaskRepository, IEnumerable<DbTask>>(x =>
                x.Find(filter, skipCount, takeCount, out totalCount), Times.Once);
        }
    }
}
