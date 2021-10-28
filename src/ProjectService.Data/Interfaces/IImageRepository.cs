using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface IImageRepository
    {
        Task<List<Guid>> CreateAsync(List<DbProjectImage> images);

        Task<bool> RemoveAsync(List<Guid> imagesIds);
    }
}
