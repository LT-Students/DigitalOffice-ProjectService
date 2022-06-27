using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Department;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.File;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Image;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Time;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Publishes
{
  public class Publish : IPublish
  {
    private readonly IBus _bus;

    public Publish(
      IBus bus)
    {
      _bus = bus;
    }

    public async Task CreateDepartmentEntityAsync(Guid departmentId, Guid createdBy, Guid projectId)
    {
      await _bus.Publish<ICreateDepartmentEntityPublish>(ICreateDepartmentEntityPublish.CreateObj(
        departmentId: departmentId,
        createdBy: createdBy,
        projectId: projectId));
    }

    public async Task CreateWorkTimeAsync(Guid projectId, List<Guid> usersIds)
    {
      await _bus.Publish<ICreateWorkTimePublish>(ICreateWorkTimePublish.CreateObj(projectId, usersIds));
    }

    public async Task RemoveImagesAsync(List<Guid> imagesIds)
    {
      await _bus.Publish<IRemoveImagesPublish>(IRemoveImagesPublish.CreateObj(
        imagesIds: imagesIds,
        imageSource: ImageSource.Project));
    }

    public async Task RemoveFilesAsync(List<Guid> filesIds)
    {
      await _bus.Publish<IRemoveFilesPublish>(IRemoveFilesPublish.CreateObj(filesIds));
    }
  }
}
