using LT.DigitalOffice.Kernel.Configurations;

namespace LT.DigitalOffice.ProjectService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string GetFileEndpoint { get; set; }
        public string GetUserDataEndpoint { get; set; }
        public string GetDepartmentDataEndpoint { get; set; }
    }
}
