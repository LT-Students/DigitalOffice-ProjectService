using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LT.DigitalOffice.ProjectService.Broker.UnitTests.GetProjectUserInfoConsumerTests;

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests
{
    class GetProjectUserInfoConsumerTests
    {
        public class GetProjectUserResponse : IGetProjectUserResponse
        {
            public Guid Id { get; set; }
            public bool IsActive { get; set; }
        }

        private InMemoryTestHarness harness;
        private Mock<IProjectRepository> projectRepositoryMock;
        private IEnumerable<DbProjectUser> projectUsers;
        private ConsumerTestHarness<GetProjectUserInfoConsumer> consumerTestHarness;

        private Guid projectId = Guid.NewGuid();
        private Guid userId = Guid.NewGuid();

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            harness = new InMemoryTestHarness();
            projectRepositoryMock = new Mock<IProjectRepository>();

            projectUsers = new List<DbProjectUser>
            {
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProjectId = projectId,
                    IsActive = true
                },
                new DbProjectUser
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    ProjectId = projectId,
                    IsActive = true
                }
            };

            projectRepositoryMock
                .Setup(x => x.GetProjectUsers(projectId, true))
                .Returns(projectUsers);

            consumerTestHarness = harness.Consumer(
                () => new GetProjectUserInfoConsumer(projectRepositoryMock.Object));
        }
        #endregion

        [Test]
        public async Task ShouldSendResponseToBrokerWhenProjectUserFoundInDb()
        {
            var expectedResponse = new GetProjectUserResponse
            {
                Id = projectUsers.First().Id,
                IsActive = projectUsers.First().IsActive
            };

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetProjectUserRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetProjectUserResponse>>(
                    IGetProjectUserRequest.CreateObj(projectId, userId));

                Assert.True(response.Message.IsSuccess);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
                Assert.That(consumerTestHarness.Consumed.Select<IGetProjectUserRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IGetProjectUserResponse>>().Any(), Is.True);
                projectRepositoryMock.Verify(repository => repository.GetProjectUsers(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task ShouldExceptionWhenProjectUserNotFoundInDb()
        {
            GetProjectUserResponse expectedResponse = null;

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetProjectUserRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetProjectUserResponse>>(IGetProjectUserRequest.CreateObj(projectId, Guid.NewGuid()));

                Assert.False(response.Message.IsSuccess);
                SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
                Assert.That(consumerTestHarness.Consumed.Select<IGetProjectUserRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IGetProjectUserResponse>>().Any(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
