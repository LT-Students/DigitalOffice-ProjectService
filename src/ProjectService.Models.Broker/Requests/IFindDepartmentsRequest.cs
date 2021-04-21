using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.FindDepartmentEndpoint))]
    public interface IFindDepartmentsRequest
    {
        string DepartmentName { get; set; }

        static object CreateObj(string departmentName)
        {
            return new
            {
                DepartmentName = departmentName
            };
        }
    }
}
