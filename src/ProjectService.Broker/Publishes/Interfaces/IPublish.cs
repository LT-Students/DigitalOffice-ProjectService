using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.ProjectService.Broker.Publishes.Interfaces
{
  [AutoInject]
  public interface IPublish
  {
    Task CreateWorkTimeAsync(Guid projectId, List<Guid> usersIds);
    Task RemoveImagesAsync(List<Guid> imagesIds);
    Task RemoveFilesAsync(List<Guid> filesIds);
  }
}
