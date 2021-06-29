using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;

namespace LT.DigitalOffice.ProjectService.Data
{
    public class TaskPropertyRepository : ITaskPropertyRepository
    {
        private readonly IDataProvider _provider;

        public TaskPropertyRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public void Create(IEnumerable<DbTaskProperty> dbTaskProperty)
        {
            _provider.TaskProperties.AddRange(dbTaskProperty);
            _provider.Save();
        }

        public bool AreExist(params Guid[] ids)
        {
            var dbIds = _provider.TaskProperties.Select(x => x.Id);

            return ids.All(x => dbIds.Contains(x));
        }

        public bool AreExistForProject(Guid projectId, params string[] propertyNames)
        {
            var dbPropertyNames = _provider.TaskProperties
                .Where(tp => tp.ProjectId == projectId)
                .Select(x => x.Name);

            return propertyNames.Any(x => dbPropertyNames.Contains(x));
        }

        public DbTaskProperty Get(Guid propertyId)
        {
            return _provider.TaskProperties.FirstOrDefault(x => x.Id == propertyId) ??
                   throw new NotFoundException($"Property with id: '{propertyId}' was not found.");
        }

        public IEnumerable<DbTaskProperty> Find(FindTaskPropertiesFilter filter, int skipCount, int takeCount, out int totalCount)
        {
            var dbTaskProperties = _provider.TaskProperties.AsQueryable();

            if (filter.ProjectId.HasValue)
            {
                dbTaskProperties = dbTaskProperties.Where(tp => tp.ProjectId == filter.ProjectId.Value);
            }

            if (filter.AuthorId.HasValue)
            {
                dbTaskProperties = dbTaskProperties.Where(tp => tp.ProjectId == filter.AuthorId.Value);
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                dbTaskProperties = dbTaskProperties.Where(tp => tp.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }

            totalCount = dbTaskProperties.Count();

            return dbTaskProperties.Skip(skipCount * takeCount).Take(takeCount).ToList();
        }
    }
}
