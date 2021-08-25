using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Time;
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

        [AutoInjectRequest(typeof(IGetImagesRequest))]
        public string GetImagesEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetDepartmentRequest))]
        public string GetDepartmentEndpoint { get; set; }

        [AutoInjectRequest(typeof(IFindDepartmentsRequest))]
        public string FindDepartmentsEndpoint { get; set; }

        [AutoInjectRequest(typeof(ICreateWorkspaceRequest))]
        public string CreateWorkspaceEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetUsersDepartmentsUsersPositionsRequest))]
        public string GetUsersDepartmentsUsersPositionsEndpoint { get; set; }

        [AutoInjectRequest(typeof(ICheckUsersExistence))]
        public string CheckUsersExistenceEndpoint { get; set; }

        [AutoInjectRequest(typeof(ICreateWorkTimeRequest))]
        public string CreateWorkTimeEndpoint { get; set; }

        [AutoInjectRequest(typeof(IGetImagesProjectRequest))]
        public string GetImagesProjectEndpoint { get; set; }

        [AutoInjectRequest(typeof(ICreateImagesProjectRequest))]
        public string CreateImagesProjectEndpoint { get; set; }

        [AutoInjectRequest(typeof(IDeleteImagesProjectRequest))]
        public string DeleteImagesProjectEndpoint { get; set; }

        [AutoInjectRequest(typeof(ICheckDepartmentsExistence))]
        public string CheckDepartmentsExistenceEndpoint { get; set; }

        public string GetProjectIdsEndpoint { get; set; }

        public string GetProjectInfoEndpoint { get; set; }

        public string GetDepartmentProjectsEndpoint { get; set; }

        public string GetUserProjectsInfoEndpoint { get; set; }

        public string SearchProjectsEndpoint { get; set; }

        public string FindProjectsEndpoint { get; set; }

        public string FindParseEntitiesEndpoint { get; set; }

        public string GetProjectsUsersEndpoint { get; set; }

        public string DisactivateUserEndpoint { get; set; }

        public string GetProjectUsersEndpoint { get; set; }

    }
}
