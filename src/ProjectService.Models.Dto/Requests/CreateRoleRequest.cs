using LT.DigitalOffice.ProjectService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Requests
{
    public class CreateRoleRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
