using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels
{
    class RoleRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
