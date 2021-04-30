using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
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
        private Dictionary<Guid, string> _idNameDepartment;

        private const string _name = "Name";

        [SetUp]
        public void SetUp()
        {
            _mapper = new FindDbProjectFilterMapper();

            _filter = new FindProjectsFilter
            {
                Name = _name,
                DepartmentName = "DepName",
                ShortName = "SH"
            };

            _departmentId = Guid.NewGuid();

            _idNameDepartment = new();
            _idNameDepartment.Add(_departmentId, _name);

            _resultFilter = new FindDbProjectsFilter
            {
                Name = _name,
                IdNameDepartments = _idNameDepartment,
                ShortName = "SH"
            };

        }

        [Test]
        public void ShouldReturnFindDbProjectsFilter()
        {
            SerializerAssert.AreEqual(_resultFilter, _mapper.Map(_filter, _idNameDepartment));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFindProjectsFilterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, _idNameDepartment));
        }
    }
}
