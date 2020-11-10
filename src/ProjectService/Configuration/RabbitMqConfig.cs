using LT.DigitalOffice.Kernel.Broker;

namespace LT.DigitalOffice.ProjectService.Configuration
{
    public class RabbitMqConfig : BaseRabbitMqOptions
    {
        public string FileServiceUrl { get; set; }
        public string UserServiceUsersUrl { get; set; }
        public string CompanyServiceDepartmentsUrl { get; set; }
    }
}
