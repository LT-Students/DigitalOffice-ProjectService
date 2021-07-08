using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Mappers.Helpers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.RequestsMappers
{
    internal class DbTaskMapperTests
    {
        private IDbTaskMapper _dbTaskMapper;
        private CreateTaskRequest _createTaskRequest;
        private AutoMocker _mocker;

        private readonly Guid authorId = Guid.NewGuid();

        private static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;

        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mocker = new AutoMocker();

            _dbTaskMapper = new DbTaskMapper();

            _createTaskRequest = new CreateTaskRequest
            {
                Name = "Create Smth",
                Description = "Create smth in somewhere",
                PlannedMinutes = 30,
                AssignedTo = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid(),
                TypeId = Guid.NewGuid()
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenCreateTaskRequestIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _dbTaskMapper.Map(null, authorId));
        }

        [Test]
        public void ShouldReturnDbTaskWhenCreateTaskRequestIsMapped()
        {
            var project = new List<DbProject>
            {
                new DbProject { Id = _createTaskRequest.ProjectId}
            };

            int maxNumber = 3;

            var tasks = new List<DbTask>
            {
                new DbTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = _createTaskRequest.ProjectId,
                    Number = maxNumber
                }
            };

            _mocker
                .Setup<IDataProvider, DbSet<DbProject>>(x => x.Projects)
                .Returns(GetQueryableMockDbSet(project));

            _mocker
                .Setup<IDataProvider, DbSet<DbTask>>(x => x.Tasks)
                .Returns(GetQueryableMockDbSet(tasks));

            TaskNumberHelper.LoadCache(_mocker.GetMock<IDataProvider>().Object);
            var dbTask = _dbTaskMapper.Map(_createTaskRequest, authorId);
            var expectedDbTask = new DbTask
            {
                Id = dbTask.Id,
                Name = _createTaskRequest.Name,
                Description = _createTaskRequest.Description,
                PlannedMinutes = _createTaskRequest.PlannedMinutes,
                AssignedTo = _createTaskRequest.AssignedTo,
                AuthorId = authorId,
                ProjectId = _createTaskRequest.ProjectId,
                CreatedAt = dbTask.CreatedAt,
                ParentId = _createTaskRequest.ParentId,
                Number = maxNumber + 1,
                PriorityId = _createTaskRequest.PriorityId,
                StatusId = _createTaskRequest.StatusId,
                TypeId = _createTaskRequest.TypeId
            };

            SerializerAssert.AreEqual(expectedDbTask, dbTask);
        }
    }
}
