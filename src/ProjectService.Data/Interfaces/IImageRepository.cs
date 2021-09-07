using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface IImageRepository
    {
        bool Create(IEnumerable<DbProjectImage> images);

        bool Remove(IEnumerable<Guid> imagesIds);
    }
}
