using LT.DigitalOffice.ProjectService.Mappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LT.DigitalOffice.ProjectService.Mappers.ResponsesMappers.Interfaces
{
    public interface IRolesResponseMapper : IMapper<IEnumerable<DbRole>, RolesResponse>
    {
        RolesResponse Map(IEnumerable<DbRole> dbRoles);
    }
}
