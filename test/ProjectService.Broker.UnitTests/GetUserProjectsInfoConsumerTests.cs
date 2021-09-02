//using LT.DigitalOffice.Kernel.Broker;
//using LT.DigitalOffice.Models.Broker.Models;
//using LT.DigitalOffice.Models.Broker.Requests.Project;
//using LT.DigitalOffice.Models.Broker.Responses.Project;
//using LT.DigitalOffice.ProjectService.Data.Interfaces;
//using LT.DigitalOffice.ProjectService.Models.Db;
//using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
//using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
//using LT.DigitalOffice.UnitTestKernel;
//using MassTransit.Testing;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LT.DigitalOffice.ProjectService.Broker.UnitTests
//{
//    class GetUserProjectsInfoConsumerTests
//    {
//        private InMemoryTestHarness _harness;
//        private ConsumerTestHarness<GetUserProjectsInfoConsumer> _consumer;
//        private Mock<IUserRepository> _repository;
//        private Guid _userId = Guid.NewGuid();
//        private List<DbProjectUser> _userProjects;

//        [SetUp]
//        public void SetUp()
//        {
//            _repository = new Mock<IUserRepository>();

//            _userProjects = new List<DbProjectUser>();
//            _userProjects.Add(new DbProjectUser
//            {
//                Project = new DbProject
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Name1",
//                    Status = 1,
//                }
//            });
//            _userProjects.Add(new DbProjectUser
//            {
//                Project = new DbProject
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Name2",
//                    Status = 1,
//                }
//            });

//            _harness = new InMemoryTestHarness();
//            _consumer = _harness.Consumer(() =>
//                new GetUserProjectsInfoConsumer(_repository.Object));
//        }

//        [Test]
//        public async Task SuccessResponce()
//        {
//            _repository
//                .Setup(x => x.Find(It.IsAny<FindDbProjectsUserFilter>()))
//                .Returns(_userProjects);

//            await _harness.Start();

//            try
//            {
//                var requestClient = await _harness.ConnectRequestClient<IGetUserProjectsInfoRequest>();

//                var response = await requestClient.GetResponse<IOperationResult<IGetUserProjectsInfoResponse>>(
//                    IGetUserProjectsInfoRequest.CreateObj(_userId), default, default);

//                var expectedResult = new
//                {
//                    IsSuccess = true,
//                    Errors = null as List<string>,
//                    Body = IGetUserProjectsInfoResponse.CreateObj(new List<ProjectShortInfo>
//                    {
//                        new ProjectShortInfo(
//                            _userProjects[0].Project.Id,
//                            _userProjects[0].Project.Name,
//                            ((ProjectStatusType)_userProjects[0].Project.Status).ToString()),
//                        new ProjectShortInfo(
//                            _userProjects[1].Project.Id,
//                            _userProjects[1].Project.Name,
//                            ((ProjectStatusType)_userProjects[1].Project.Status).ToString())
//                    })
//                };

//                Assert.True(response.Message.IsSuccess);
//                Assert.AreEqual(null, response.Message.Errors);
//                SerializerAssert.AreEqual(expectedResult, response.Message);
//                Assert.True(_harness.Sent.Select<IOperationResult<IGetUserProjectsInfoResponse>>().Any());
//                _repository.Verify(x => x.Find(It.IsAny<FindDbProjectsUserFilter>()), Times.Once);
//            }
//            finally
//            {
//                await _harness.Stop();
//            }
//        }
//    }
//}
