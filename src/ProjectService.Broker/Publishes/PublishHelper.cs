using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Department;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Time;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Publishes
{
  public class PublishHelper : IPublishHelper
  {
    private readonly IBus _bus;

    public PublishHelper(
      IBus bus)
    {
      _bus = bus;
    }

    public async Task CreateDepartmentEntityPublish(Guid departmentId, Guid createdBy, Guid? userId = null, Guid? projectId = null)
    {
      await _bus.Publish<ICreateDepartmentEntityPublish>(ICreateDepartmentEntityPublish.CreateObj(
        departmentId: departmentId,
        createdBy: createdBy,
        userId: userId,
        projectId: projectId));
    }

    public async Task CreateWorkTimePublish(Guid projectId, List<Guid> usersIds)
    {
      await _bus.Publish<ICreateWorkTimePublish>(ICreateWorkTimePublish.CreateObj(projectId, usersIds));
    }
  }
}
