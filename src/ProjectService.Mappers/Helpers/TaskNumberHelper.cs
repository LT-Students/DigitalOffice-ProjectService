using LT.DigitalOffice.ProjectService.Data.Provider;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Mappers.Helpers
{
    public static class TaskNumberHelper
    {
        private static ConcurrentDictionary<Guid, int> _cache = new();

        public static void LoadCache(IDataProvider provider)
        {
            foreach (var projectId in provider.Projects.Select(x => x.Id))
            {
                _cache[projectId] = provider.Tasks?.Where(x => x.ProjectId == projectId)?.Max(n => n.Number) ?? 0;
            }
        }

        public static int GetProjectTaskNumber(Guid projectId)
        {
            return _cache.ContainsKey(projectId) ? _cache[projectId] += 1 : _cache[projectId] = 1;
        }
    }
}
