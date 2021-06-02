using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
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

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests
{
    public class SearchProjectsConsumerTests
    {
        private ConsumerTestHarness<SearchProjectsConsumer> _consumerTestHarness;

        private InMemoryTestHarness _harness;
        private List<DbProject> _dbProjects;
        private List<SearchInfo> _result;

        private const string ExistName = "SH";

        private Mock<IProjectRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _harness = new InMemoryTestHarness();
            _consumerTestHarness = _harness.Consumer(() =>
                new SearchProjectsConsumer(_repository.Object));

            _dbProjects = new()
            {
                new DbProject
                {
                    Id = Guid.NewGuid(),
                    ShortName = "SH1"
                },
                new DbProject
                {
                    Id = Guid.NewGuid(),
                    ShortName = "SH2"
                },
                new DbProject
                {
                    Id = Guid.NewGuid(),
                    ShortName = "Sawf3",
                    Name = "SHname"
                }
            };

            _result = new()
            {
                new SearchInfo(_dbProjects[0].Id, _dbProjects[0].ShortName),
                new SearchInfo(_dbProjects[1].Id, _dbProjects[1].ShortName),
                new SearchInfo(_dbProjects[2].Id, _dbProjects[2].Name)
            };

            _repository = new();
            _repository
                .Setup(r => r.Search(ExistName))
                .Returns(_dbProjects);
        }

        [Test]
        public async Task ShouldConsumeSuccessful()
        {
            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<ISearchProjectsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<ISearchProjectsResponse>>(
                    ISearchProjectsRequest.CreateObj(ExistName), default, default);

                var expectedResult = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new
                    {
                        Projects = _result
                    }
                };

                SerializerAssert.AreEqual(expectedResult, response.Message);
                Assert.True(_consumerTestHarness.Consumed.Select<ISearchProjectsRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<ISearchProjectsResponse>>().Any());
                _repository.Verify(x => x.Search(ExistName), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }

        [Test]
        public async Task ShouldThrowExceptionWhenRepositoryThrow()
        {
            _repository
                .Setup(r => r.Search(ExistName))
                .Throws(new Exception());

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<ISearchProjectsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<ISearchProjectsResponse>>(
                    ISearchProjectsRequest.CreateObj(ExistName), default, default);

                var expectedResult = new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "some error" },
                };

                Assert.IsFalse(response.Message.IsSuccess);
                Assert.IsNotEmpty(response.Message.Errors);
                Assert.True(_consumerTestHarness.Consumed.Select<ISearchProjectsRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<ISearchProjectsResponse>>().Any());
                _repository.Verify(x => x.Search(ExistName), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }
    }
}
