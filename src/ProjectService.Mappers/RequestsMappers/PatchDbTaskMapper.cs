using System;
using LT.DigitalOffice.Kernel.Exceptions.Models;
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
                throw new BadRequestException();
            }

            var result = new JsonPatchDocument<DbTask>();

            foreach (var item in request.Operations)
            {
                string operation = item.OperationType switch
                {
                    OperationType.Add => "add",
                    OperationType.Replace => "replace",
                    _ => null
                };

                if (operation != null)
                {
                    result.Operations.Add(new Operation<DbTask>(operation, item.path, item.from, item.value));
                } 
            }

            return result;
        }
    }
}