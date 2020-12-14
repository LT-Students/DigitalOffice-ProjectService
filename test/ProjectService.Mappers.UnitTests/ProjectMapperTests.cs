using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests
{
    internal class ProjectModelTests
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? ClosedReason { get; set; }
        public string Description { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }

    internal class ProjectMapperTests
    {
        private const string Name = "Test Project";
        private const string Description = "DigitalOffice project. The students do the work. Sometimes. Never (c) Spartak";
        private const string ShortName = "TP";

        private IProjectMapper mapper;

        private Guid projectId;

        private DbProject dbProject;

        private DateTime createdAt;

        [SetUp]
        public void SetUp()
        {
            mapper = new ProjectMapper();

            projectId = Guid.NewGuid();

            createdAt = DateTime.Now;

            dbProject = new DbProject
            {
                Id = projectId,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                CreatedAt = createdAt,
                IsActive = false
            };
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbProjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mapper.Map(null));
        }

        [Test]
        public void ShouldReturnProjectModelWhenDbProjectIsMapped()
        {
            var result = mapper.Map(dbProject);

            var resultProjectModel = new ProjectModelTests
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
                ShortName = result.ShortName,
                CreatedAt = result.CreatedAt,
                IsActive = result.IsActive
            };

            var expected = new ProjectModelTests
            {
                Id = projectId,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                CreatedAt = createdAt,
                IsActive = false
            };

            SerializerAssert.AreEqual(expected, resultProjectModel);
        }
    }
}