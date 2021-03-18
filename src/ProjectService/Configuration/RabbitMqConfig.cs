using LT.DigitalOffice.Kernel.Broker;

namespace LT.DigitalOffice.ProjectService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqOptions
    {
        public string GetFileEndpoint { get; set; }
        public string GetUserDataEndpoint { get; set; }
        public string GetDepartmentDataEndpoint { get; set; }
    }
}
