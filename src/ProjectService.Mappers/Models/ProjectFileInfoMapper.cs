﻿using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.Models
{
    public class ProjectFileInfoMapper : IProjectFileInfoMapper
    {
        public ProjectFileInfo Map(DbProjectFile dbProjectFile)
        {
            if (dbProjectFile == null)
            {
                throw new ArgumentNullException(nameof(dbProjectFile));
            }

            return new ProjectFileInfo
            {
                FileId = dbProjectFile.FileId,
                ProjectId = dbProjectFile.ProjectId
            };
        }
    }
}
