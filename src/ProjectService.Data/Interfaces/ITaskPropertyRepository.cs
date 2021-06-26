using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface ITaskPropertyRepository
    {
        void Create(IEnumerable<DbTaskProperty> dbTaskProperties);

        bool AreExist(params Guid[] ids);

        DbTaskProperty Get(Guid propertyId);

        IEnumerable<DbTaskProperty> Find(Guid? projectId, string name, int skipCount, int tackeCount, out int totalCount);
    }
}
