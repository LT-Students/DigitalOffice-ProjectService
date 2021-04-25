using LT.DigitalOffice.Kernel.Configurations;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string GetUserDataEndpoint { get; set; }
        public string GetFileEndpoint { get; set; }
        public string GetDepartmentEndpoint { get; set; }
        public string FindDepartmentsEndpoint { get; set; }
    }
}
