using LT.DigitalOffice.ProjectService.Models.Dto.Enums;

namespace LT.DigitalOffice.ProjectService.Models.Dto.ResponsesModels
{
    public class ProjectUserInfo
    {
        public UserInfo User { get; set; }
        public UserRoleType Role { get; set; }
    }
}
