using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Data.Interfaces
{
    [AutoInject]
    public interface ITaskPropertyRepository
    {
        void Create(IEnumerable<DbTaskProperty> dbTaskProperties);

        bool AreExist(params Guid[] ids);

        bool AreExistForProject(Guid projectId, params string[] propertyNames);

        DbTaskProperty Get(Guid propertyId);

        IEnumerable<DbTaskProperty> Find(FindTaskPropertiesFilter filter, int skipCount, int takeCount, out int totalCount);
    }
}
