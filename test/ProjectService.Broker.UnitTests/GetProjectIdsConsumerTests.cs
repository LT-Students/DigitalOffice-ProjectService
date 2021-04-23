using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Broker.Requests;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests
{
    class GetProjectIdsConsumerTests
    {
        private Guid _userId = Guid.NewGuid();
        private ConsumerTestHarness<GetProjectIdsConsumer> _consumerTestHarness;

        private InMemoryTestHarness _harness;
        private List<DbProjectUser> _dbProjectUsers;

        private Mock<IUserRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IUserRepository>();

            _harness = new InMemoryTestHarness();
            _consumerTestHarness = _harness.Consumer(() =>
                new GetProjectIdsConsumer(_repository.Object));

            _dbProjectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = Guid.NewGuid(),
                    UserId = _userId,
                    RoleId = Guid.NewGuid(),
                    AddedOn = DateTime.Now,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    ProjectId = Guid.NewGuid(),
                    UserId = _userId,
                    RoleId = Guid.NewGuid(),
                    AddedOn = DateTime.Now,
                    IsActive = true
                },
            };
        }

        [Test]
        public async Task ShouldResponseProjectIdsResponse()
        {
            IList<Guid> projectIds = new List<Guid>();
            foreach (var pu in _dbProjectUsers)
            {
                projectIds.Add(pu.ProjectId);
            };

            _repository
                .Setup(x => x.Get(_userId))
                .Returns(_dbProjectUsers)
                .Verifiable();

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IGetUserProjectsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IProjectsResponse>>(
                    IGetUserProjectsRequest.CreateObj(_userId), default, default);

                var expectedResult = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new
                    {
                        ProjectsIds = projectIds
                    }
                };

                Assert.True(response.Message.IsSuccess);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResult, response.Message);
                Assert.True(_consumerTestHarness.Consumed.Select<IGetUserProjectsRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<IProjectsResponse>>().Any());
                _repository.Verify(x => x.Get(_userId), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }

        [Test]
        public async Task ShouldThrowExceptionWhenUserIdWasNotFound()
        {
            List<DbProjectUser> dbProjectUsers = null;

            _repository
                .Setup(x => x.Get(_userId))
                .Returns(dbProjectUsers);

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IGetUserProjectsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IProjectsResponse>>(
                    IGetUserProjectsRequest.CreateObj(_userId), default, default);

                var expectedResult = new
                {
                    IsSuccess = false,
                    Errors = new List<string> { $"User with id: {_userId} was not found." },
                    Body = null as object
                };

                Assert.False(response.Message.IsSuccess);
                Assert.AreEqual(expectedResult.Errors, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResult, response.Message);
                Assert.True(_consumerTestHarness.Consumed.Select<IGetUserProjectsRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<IProjectsResponse>>().Any());
                _repository.Verify(x => x.Get(_userId), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }
    }
}
