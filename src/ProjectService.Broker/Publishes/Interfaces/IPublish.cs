using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.File;

namespace LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces
{
  [AutoInject]
  public interface IPublish
  {
    Task CreateFilesAsync(List<FileData> files, Guid createdBy);
  }
}
