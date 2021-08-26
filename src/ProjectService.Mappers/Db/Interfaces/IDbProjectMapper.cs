using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces
{
    [AutoInject]
    public interface IDbProjectMapper
    {
        DbProject Map(ProjectRequest request, Guid authorId, List<Guid> users, List<Guid> departmentIds, List<Guid> imagesIds);
    }
}
