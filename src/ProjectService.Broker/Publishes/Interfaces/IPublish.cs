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
    Task CreateDepartmentEntityAsync(Guid departmentId, Guid createdBy, Guid projectId);
    Task CreateWorkTimeAsync(Guid projectId, List<Guid> usersIds);
    Task RemoveImagesAsync(List<Guid> imagesIds);
  }
}
