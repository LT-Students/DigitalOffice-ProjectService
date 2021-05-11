using System;
using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class PatchDbTaskMapper : IPatchDbTaskMapper
    {
        public JsonPatchDocument<DbTask> Map(JsonPatchDocument<EditTaskRequest> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            var result = new JsonPatchDocument<DbTask>();

            foreach (var item in request.Operations)
            {
                result.Operations.Add(new Operation<DbTask>(item.op, item.path, item.from, item.value));
            }

            return result;
        }
    }
}
