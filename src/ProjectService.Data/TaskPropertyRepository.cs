using System;
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
            return ids.All(id => _provider.TaskProperties.FirstOrDefault(x => x.Id == id) != null);
        }

        public DbTaskProperty Get(Guid propertyId)
        {
            return _provider.TaskProperties.FirstOrDefault(x => x.Id == propertyId) ??
                   throw new NotFoundException($"Property with id: '{propertyId}' was not found.");
        }
    }
}