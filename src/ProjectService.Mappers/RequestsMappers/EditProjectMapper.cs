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

            JsonPatchDocument<DbProject> patchDbProject = new();

            foreach (var item in request.Operations)
            {
                if (item.op == "replace")
                {
                    patchDbProject.Operations.Add(new Operation<DbProject>(item.op, item.path, item.from, item.value));
                }
                else
                {
                    throw new ArgumentException("Invalid operation");
                }
            }

            return patchDbProject;
        }
    }
}
