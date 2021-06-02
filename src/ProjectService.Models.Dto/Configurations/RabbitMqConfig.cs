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

        [AutoInjectRequest(typeof(IGetUserProjectsRequest))]
        public string GetProjectIdsEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetProjectRequest))]
        public string GetProjectInfoEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUserProjectsInfoRequest))]
        public string GetUserProjectsInfoEndpoint { get; set; }

        [AutoInjectRequest(typeof(ISearchProjectsRequest))]
        public string SearchProjectsEndpoint { get; set; }
    }
}
