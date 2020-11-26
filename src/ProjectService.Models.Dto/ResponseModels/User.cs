using LT.DigitalOffice.ProjectService.Models.Dto.RequestModels;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponseModels
{
    public class User : UserRequest
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
    }
}
