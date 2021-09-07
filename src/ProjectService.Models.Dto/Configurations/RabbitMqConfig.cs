using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Time;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string SearchProjectsEndpoint { get; set; }
        public string FindParseEntitiesEndpoint { get; set; }
        public string GetProjectsUsersEndpoint { get; set; }
        public string GetProjectsEndpoint { get; set; }
        public string DisactivateUserEndpoint { get; set; }

        // file

        [AutoInjectRequest(typeof(IGetFileRequest))]
        public string GetFileEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetImagesRequest))]
        public string GetImagesEndpoint { get; set; }

        // company

        [AutoInjectRequest(typeof(IGetDepartmentsRequest))]
        public string GetDepartmentsEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetCompanyEmployeesRequest))]
        public string GetCompanyEmployeesEndpoint { get; set; }

        [AutoInjectRequest(typeof(ICheckDepartmentsExistence))]
        public string CheckDepartmentsExistenceEndpoint { get; set; }

        // message

        [AutoInjectRequest(typeof(ICreateWorkspaceRequest))]
        public string CreateWorkspaceEndpoint { get; set; }

        // user

        [AutoInjectRequest(typeof(ICheckUsersExistence))]
        public string CheckUsersExistenceEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUserDataRequest))]
        public string GetUserDataEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersDataRequest))]
        public string GetUsersDataEndpoint { get; set; }

        // time

        [AutoInjectRequest(typeof(ICreateWorkTimeRequest))]
        public string CreateWorkTimeEndpoint { get; set; }
    }
}
