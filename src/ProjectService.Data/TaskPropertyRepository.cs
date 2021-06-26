using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskPropertyRepository : ITaskPropertyRepository
    {
        private readonly IDataProvider _provider;

        public TaskPropertyRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public bool AreExist(params Guid[] ids)
        {
            var dbIds = _provider.TaskProperties.Select(x => x.Id);

            return ids.All(x => dbIds.Contains(x));
        }

        public DbTaskProperty Get(Guid propertyId)
        {
            return _provider.TaskProperties.FirstOrDefault(x => x.Id == propertyId) ??
                   throw new NotFoundException($"Property with id: '{propertyId}' was not found.");
        }

        public IEnumerable<DbTaskProperty> Find(Guid? projectId, string name, int skipCount, int tackeCount, out int totalCount)
        {
            var dbTaskProperties = _provider.TaskProperties.AsQueryable();

            if (projectId.HasValue)
            {
                dbTaskProperties = dbTaskProperties.Where(tp => tp.ProjectId == projectId.Value);
            }

            if (!string.IsNullOrEmpty(name))
            {
                dbTaskProperties = dbTaskProperties.Where(tp => tp.Name.Contains(name));
            }

            totalCount = dbTaskProperties.Count();

            return dbTaskProperties.Skip(skipCount * tackeCount).Take(tackeCount).ToList();
        }
    }
}
