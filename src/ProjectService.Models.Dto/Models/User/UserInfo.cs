using LT.DigitalOffice.ProjectService.Models.Dto.RequestsModels;
using System;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class UserInfo : UserRequest
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
    }
}
