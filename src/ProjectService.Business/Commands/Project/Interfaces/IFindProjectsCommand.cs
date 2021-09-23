﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;

namespace LT.DigitalOffice.ProjectService.Business.Commands.Project.Interfaces
{
    [AutoInject]
    public interface IFindProjectsCommand
    {
        FindResponse<ProjectInfo> Execute(FindProjectsFilter filter);
    }
}
