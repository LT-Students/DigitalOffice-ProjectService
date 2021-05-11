﻿using LT.DigitalOffice.ProjectService.Mappers.ModelsMappers.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Mappers.ModelsMappers
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