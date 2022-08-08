using LT.DigitalOffice.Kernel.BrokerSupport.Attributes;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Configurations
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    public string SearchProjectsEndpoint { get; set; }
    public string FindParseEntitiesEndpoint { get; set; }
    public string GetProjectsUsersEndpoint { get; set; }
    public string GetProjectsEndpoint { get; set; }
    public string DisactivateProjectUserEndpoint { get; set; }
    public string CheckProjectsExistenceEndpoint { get; set; }
    public string CheckProjectUsersExistenceEndpoint { get; set; }
    public string CheckFilesAccessesEndpoint { get; set; }
    public string CreateFilesEndpoint { get; set; }
    public string GetProjectUserRoleEndpoint { get; set; }

    // positions

    [AutoInjectRequest(typeof(IGetPositionsRequest))]
    public string GetPositionsEndpoint { get; set; }

    // image

    [AutoInjectRequest(typeof(IGetImagesRequest))]
    public string GetImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateImagesRequest))]
    public string CreateImagesEndpoint { get; set; }

    // department

    [AutoInjectRequest(typeof(IGetDepartmentsRequest))]
    public string GetDepartmentsEndpoint { get; set; }

    // office

    [AutoInjectRequest(typeof(IGetOfficesRequest))]
    public string GetOfficesEndpoint { get; set; }

    // message

    [AutoInjectRequest(typeof(ICreateWorkspaceRequest))]
    public string CreateWorkspaceEndpoint { get; set; }

    // user

    [AutoInjectRequest(typeof(ICheckUsersExistence))]
    public string CheckUsersExistenceEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetUsersDataRequest))]
    public string GetUsersDataEndpoint { get; set; }

    [AutoInjectRequest(typeof(IFilteredUsersDataRequest))]
    public string FilterUsersDataEndpoint { get; set; }

    // file

    [AutoInjectRequest(typeof(IGetFilesRequest))]
    public string GetFilesEndpoint { get; set; }
  }
}
