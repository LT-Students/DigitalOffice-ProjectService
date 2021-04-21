using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class Project
    {
        public new Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}