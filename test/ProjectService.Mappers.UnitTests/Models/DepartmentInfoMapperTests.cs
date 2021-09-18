//using LT.DigitalOffice.Models.Broker.Responses.Company;
//using LT.DigitalOffice.ProjectService.Mappers.Models;
//using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
//using LT.DigitalOffice.ProjectService.Models.Dto.Models;
//using LT.DigitalOffice.UnitTestKernel;
//using Moq;
//using NUnit.Framework;
//using System;

//namespace LT.DigitalOffice.ProjectService.Mappers.UnitTests.Models
//{
//    class DepartmentInfoMapperTests
//    {
//        public IGetDepartmentResponse _departmentResponse;
//        public DepartmentInfo _expectedDepartmentInfo;

//        public IDepartmentInfoMapper _mapper;

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _mapper = new DepartmentInfoMapper();

//            var departmentResponseMock = new Mock<IGetDepartmentResponse>();
//            departmentResponseMock.Setup(x => x.DepartmentId).Returns(Guid.NewGuid());
//            departmentResponseMock.Setup(x => x.Name).Returns("Some department");

//            _departmentResponse = departmentResponseMock.Object;

//            _expectedDepartmentInfo = new DepartmentInfo
//            {
//                Id = _departmentResponse.DepartmentId,
//                Name = _departmentResponse.Name
//            };
//        }

//        [Test]
//        public void ShouldThrowExceptionWhenIGetDepartmentInfoIsNull()
//        {
//            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
//        }

//        [Test]
//        public void ShouldReturnDepartmentInfoSuccessful()
//        {
//            SerializerAssert.AreEqual(_expectedDepartmentInfo, _mapper.Map(_departmentResponse));
//        }
//    }
//}
