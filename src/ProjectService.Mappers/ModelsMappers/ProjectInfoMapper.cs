using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
{
    public class ProjectInfoMapper : IProjectInfoMapper
    {
        public ProjectInfo Map(DbProject dbProject, string departmetnName)
        {
            if (dbProject == null)
            {
                throw new ArgumentNullException(nameof(dbProject));
            }

            return new ProjectInfo
            {
                Id = dbProject.Id,
                Name = dbProject.Name,
                ShortName = dbProject.ShortName,
                ShortDescription = dbProject.ShortDescription,
                DepartmentInfo = new DepartmentInfo
                {
                    Id = dbProject.DepartmentId,
                    Name = departmetnName
                }
            };
        }
    }
}
