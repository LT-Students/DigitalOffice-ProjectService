using LT.DigitalOffice.ProjectService.Data;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.ProjectServiceUnitTests.Repositories
{
    internal class EditProjectRepositoryTests
    {
        private IDataProvider _provider;
        private ProjectRepository _repository;

        private DbProject _dbProjectBefore;
        private DbProject _dbProjectAfter;
        private JsonPatchDocument<DbProject> _patchProject;
        private DbContextOptions<ProjectServiceDbContext> _dbOptionsProjectService;
        private Guid _creatorId = Guid.NewGuid();
        private Mock<IHttpContextAccessor> _accessorMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dbProjectBefore = new DbProject
            {
                Id = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
                Status = (int)ProjectStatusType.Active,
                Name = "Name",
                ShortName = "ShortName",
                Description = "Description",
                ShortDescription = "ShortDescription",
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbProjectAfter = new DbProject
            {
                Id = _dbProjectBefore.Id,
                DepartmentId = Guid.NewGuid(),
                CreatedBy = _dbProjectBefore.CreatedBy,
                Status = (int)ProjectStatusType.Closed,
                Name = "Name1",
                ShortName = "ShortName1",
                Description = "Description1",
                ShortDescription = "ShortDescription1",
                CreatedAtUtc = _dbProjectBefore.CreatedAtUtc
            };

            _patchProject = new JsonPatchDocument<DbProject>(new List<Operation<DbProject>>
            {
                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.Name)}",
                    "",
                    $"{_dbProjectAfter.Name}"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.ShortName)}",
                    "",
                    $"{_dbProjectAfter.ShortName}"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.Description)}",
                    "",
                    $"{_dbProjectAfter.Description}"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.ShortDescription)}",
                    "",
                    $"{_dbProjectAfter.ShortDescription}"),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.Status)}",
                    "",
                    _dbProjectAfter.Status),

                new Operation<DbProject>(
                    "replace",
                    $"/{nameof(DbProject.DepartmentId)}",
                    "",
                    _dbProjectAfter.DepartmentId)

            }, new CamelCasePropertyNamesContractResolver());
        }

        [SetUp]
        public void SetUp()
        {
            _accessorMock = new();
            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _creatorId);

            _accessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(_items);

            _dbOptionsProjectService = new DbContextOptionsBuilder<ProjectServiceDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjectServiceTest")
                .Options;

            _provider = new ProjectServiceDbContext(_dbOptionsProjectService);
            _repository = new ProjectRepository(_provider, _accessorMock.Object);
        }

        [Test]
        public void SuccessEditProject()
        {
            _provider.Projects.Add(_dbProjectBefore);
            _provider.Save();

            SerializerAssert.AreEqual(true, _repository.Edit(_dbProjectBefore.Id, _patchProject));

            var patchedProject = _provider.Projects.FirstOrDefault(p => p.Id == _dbProjectBefore.Id);
            _dbProjectAfter.ModifiedAtUtc = patchedProject.ModifiedAtUtc;
            _dbProjectAfter.ModifiedBy = patchedProject.ModifiedBy;
            SerializerAssert.AreEqual(_dbProjectAfter, patchedProject);
        }

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
