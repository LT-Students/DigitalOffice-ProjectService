using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Broker.Requests;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
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
    class GetProjectInfoConsumerTests
    {
        private Guid _projectId = Guid.NewGuid();
        private ConsumerTestHarness<GetProjectInfoConsumer> _consumerTestHarness;

        private InMemoryTestHarness _harness;
        private DbProject _dbProject;

        private Mock<IProjectRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IProjectRepository>();

            _harness = new InMemoryTestHarness();
            _consumerTestHarness = _harness.Consumer(() =>
                new GetProjectInfoConsumer(_repository.Object));

            _dbProject = new DbProject
            {
                Id = Guid.NewGuid(),
                Name = "DigitalOffice",
                ShortName = "DO",
                DepartmentId = Guid.NewGuid(),
                Description = "New project for Lanit-Tercom",
                Status = (int)ProjectStatusType.Suspend
            };
        }

        [Test]
        public async Task ShouldResponseProjectInfoResponse()
        {
            _repository
                .Setup(x => x.GetProject(_projectId))
                .Returns(_dbProject);

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IGetProjectRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IProjectResponse>>(
                    IGetProjectRequest.CreateObj(_projectId), default, default);

                var expectedResult = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new
                    {
                        Id = _dbProject.Id,
                        Name = _dbProject.Name,
                        ProjectStatus = _dbProject.Status
                    }
                };

                Assert.True(response.Message.IsSuccess);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResult, response.Message);
                Assert.True(_consumerTestHarness.Consumed.Select<IGetProjectRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<IProjectResponse>>().Any());
                _repository.Verify(x => x.GetProject(_projectId), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }

        [Test]
        public async Task ShouldThrowExceptionWhenProjectIdWasNotFound()
        {
            DbProject dbProject = null;

            _repository
                .Setup(x => x.GetProject(_projectId))
                .Returns(dbProject);

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IGetProjectRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IProjectResponse>>(
                    IGetProjectRequest.CreateObj(_projectId), default, default);

                var expectedResult = new
                {
                    IsSuccess = false,
                    Errors = new List<string> { $"Project with id: {_projectId} was not found." },
                    Body = null as object
                };

                Assert.False(response.Message.IsSuccess);
                Assert.AreEqual(expectedResult.Errors, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResult, response.Message);
                Assert.True(_consumerTestHarness.Consumed.Select<IGetProjectRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<IProjectResponse>>().Any());
                _repository.Verify(x => x.GetProject(_projectId), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }
    }
}
