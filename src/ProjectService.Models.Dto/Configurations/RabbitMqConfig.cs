using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        [AutoInjectRequest(typeof(IGetUserDataRequest))]
        public string GetUserDataEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersDataRequest))]
        public string GetUsersDataEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetFileRequest))]
        public string GetFileEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetDepartmentRequest))]
        public string GetDepartmentEndpoint { get; set; }

        [AutoInjectRequest(typeof(IFindDepartmentsRequest))]
        public string FindDepartmentsEndpoint { get; set; }

        public string GetProjectIdsEndpoint { get; set; }

        public string GetProjectInfoEndpoint { get; set; }

        public string GetUserProjectsInfoEndpoint { get; set; }

        public string SearchProjectsEndpoint { get; set; }
    }
}
