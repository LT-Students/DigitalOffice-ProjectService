using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels.Filters;
using LT.DigitalOffice.UnitTestKernel;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.RequestsMappers
{
    class FindDbProjectFilterMapperTests
    {
        private IFindDbProjectFilterMapper _mapper;
        private FindProjectsFilter _filter;
        private FindDbProjectsFilter _resultFilter;
        private Guid _departmentId;
        private List<Guid> _departmentIds;

        [SetUp]
        public void SetUp()
        {
            _mapper = new FindDbProjectFilterMapper();

            _filter = new FindProjectsFilter
            {
                Name = "Name",
                DepartmentName = "DepName",
                ShortName = "SH"
            };

            _departmentId = Guid.NewGuid();
            _departmentIds = new List<Guid> { _departmentId };

            _resultFilter = new FindDbProjectsFilter
            {
                Name = "Name",
                DepartmentIds = _departmentIds,
                ShortName = "SH"
            };
        }

        [Test]
        public void ShouldReturnFindDbProjectsFilter()
        {
            SerializerAssert.AreEqual(_resultFilter, _mapper.Map(_filter, _departmentIds));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFindProjectsFilterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, _departmentIds));
        }
    }
}
