using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers;
using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests
{
    public class ProjectMapperTests
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

            var expected = new Project
            {
                Id = projectId,
                Name = Name,
                Description = Description,
                ShortName = ShortName,
                CreatedAt = createdAt,
                IsActive = false
            };

            SerializerAssert.AreEqual(expected, result);
        }
    }
}