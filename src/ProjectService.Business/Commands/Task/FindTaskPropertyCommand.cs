﻿using LT.DigitalOffice.ProjectService.Business.Commands.Interfaces;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Models.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using LT.DigitalOffice.ProjectService.Models.Dto.Requests.Filters;
using LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels;
using System;
using System.Linq;

namespace LT.DigitalOffice.ProjectService.Business.Commands
{
    public class FindTaskPropertyCommand : IFindTaskPropertyCommand
    {
        private readonly ITaskPropertyInfoMapper _mapper;
        private readonly ITaskPropertyRepository _repository;

        public FindTaskPropertyCommand(
            ITaskPropertyRepository repository,
            ITaskPropertyInfoMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public FindResponse<TaskPropertyInfo> Execute(FindTaskPropertiesFilter filter, int skipCount, int tackeCount)
        {
            var dbTaskProperties = _repository.Find(filter, skipCount, tackeCount, out int totalCount);

            return new FindResponse<TaskPropertyInfo>
            {
                Body = dbTaskProperties.Select(tp => _mapper.Map(tp)),
                TotalCount = totalCount
            };
        }
    }
}
