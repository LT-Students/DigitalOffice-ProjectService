using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface IImageRepository
    {
        List<Guid> Create(IEnumerable<DbEntityImage> images);

        bool Remove(IEnumerable<Guid> imagesIds);
    }
}
