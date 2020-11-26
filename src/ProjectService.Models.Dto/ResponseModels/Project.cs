using LT.DigitalOffice.ProjectService.Models.Dto.RequestModels;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels
{
    public class Project : ProjectRequest
    {
        public new Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}