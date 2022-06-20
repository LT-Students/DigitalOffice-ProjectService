using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.File;
using LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Publishes
{
  
  public class Publish : IPublish
  {
    private readonly IBus _bus;

    public Publish(IBus bus)
    {
      _bus = bus;
    }
    
    public async Task CreateFilesAsync(List<FileData> files, Guid createdBy) // httpcontext take Guid
    {
      await _bus.Publish<ICreateFilesPublish>(ICreateFilesPublish.CreateObj(
      files: files,
      createdBy: createdBy));
    }
  }
}

