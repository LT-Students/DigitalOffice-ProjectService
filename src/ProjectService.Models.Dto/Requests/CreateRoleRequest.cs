using LT.DigitalOffice.ProjectService.Models.Dto;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
