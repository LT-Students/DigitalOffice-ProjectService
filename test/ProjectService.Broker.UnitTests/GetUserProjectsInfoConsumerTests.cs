using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using MassTransit.Testing;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Broker.UnitTests
{
    class GetUserProjectsInfoConsumerTests
    {
        private AutoMocker _mocker;
        private ConsumerTestHarness<GetUserProjectsInfoConsumer> _consumer;
        private List<DbProjectUser> _userProjects;
        private DbProject _dbProject1;
        private DbProject _dbProject2;

        [SetUp]
        public void SetUp()
        {
            _userProjects = new List<DbProjectUser>();
            _userProjects.Add(new DbProjectUser { ProjectId = Guid.NewGuid() });
            _userProjects.Add(new DbProjectUser { ProjectId = Guid.NewGuid() });

            _dbProject1 = new DbProject
            {
                Id = _userProjects[0].ProjectId,
                Name = "Name",
                ShortName = "ShortName",
                ShortDescription = "ShortDescription",
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };

            _dbProject2 = new DbProject
            {
                Id = _userProjects[1].ProjectId,
                Name = "Name",
                ShortName = "ShortName",
                ShortDescription = "ShortDescription",
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            _mocker = new AutoMocker();
            _mocker
                .Setup<IUserRepository, IEnumerable<DbProjectUser>>(x => x.Find(It.IsAny<Guid>()))
                .Returns(_userProjects);

            _mocker
                .Setup<IProjectRepository, DbProject>(x => x.GetProject(_userProjects[0].ProjectId))
                .Returns(_dbProject1);
            _mocker
                .Setup<IProjectRepository, DbProject>(x => x.GetProject(_userProjects[1].ProjectId))
                .Returns(_dbProject2);

        }

    }
}
