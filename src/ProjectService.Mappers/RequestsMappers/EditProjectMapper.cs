using LT.DigitalOffice.ProjectService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.RequestsMappers
{
    public class EditProjectMapper : IEditProjectMapper
    {
        public JsonPatchDocument<DbProject> Map(JsonPatchDocument<EditProjectRequest> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("Invalid request value");
            }

            JsonPatchDocument<DbProject> dbRequest = new();

            foreach (var item in request.Operations)
            {
                dbRequest.Operations.Add(new Operation<DbProject>(item.op, item.path, item.from, item.value));
            }

            return dbRequest;
        }
    }
}
