using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Provider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.ProjectService.Business.Helpers.Task
{
    public static class TaskNumber
    {
        private static ConcurrentDictionary<Guid, int> _cache = null;

        public static void LoadCache(IDataProvider provider)
        {
            var ids = provider.Projects.Select(x => x.Id);
            foreach (var Id in ids)
            {
                int? taskNumber = provider.Tasks.Where(x => x.ProjectId == Id)?.Max(n => n.Number);

                _cache[Id] = taskNumber.GetValueOrDefault();
            }
        }

        public static void GetProjectTaskMaxNumber(Guid projectId, out int result)
        {
            if (_cache.ContainsKey(projectId))
            {
                result = _cache[projectId];
            }
            else
            {
                result = _cache[projectId] = 1;
            }

            UpdateProjectTaskNumber(projectId);
        }

        private static void UpdateProjectTaskNumber(Guid projectId)
        {
            _cache[projectId] +=  1;
        }
    }
}
