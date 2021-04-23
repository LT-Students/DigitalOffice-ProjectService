using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.ProjectService.Models.Dto.Configurations;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.FindDepartmentsEndpoint))]
    public interface IFindDepartmentsRequest
    {
        string DepartmentName { get; }

        static object CreateObj(string departmentName)
        {
            return new
            {
                DepartmentName = departmentName
            };
        }
    }
}
