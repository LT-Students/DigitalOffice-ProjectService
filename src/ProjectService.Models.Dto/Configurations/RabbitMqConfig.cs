using LT.DigitalOffice.Kernel.Configurations;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string CompanyServiceUrl { get; set; }
        public string GetUserCredentialsEndpoint { get; set; }
        public string GetUserDataEndpoint { get; set; }
        public string AddImageEndpoint { get; set; }
        public string GetFileEndpoint { get; set; }
        public string GetDepartmentDataEndpoint { get; set; }
        public string GetPositionEndpoint { get; set; }
        public string ChangeUserDepartmentEndpoint { get; set; }
        public string ChangeUserPositionEndpoint { get; set; }
        public string GetProjectsEndpoint { get; set; }
        public string GetProjectEndpoint { get; set; }
        public string SendEmailEndpoint { get; set; }
        public string GetTokenEndpoint { get; set; }
    }
}
