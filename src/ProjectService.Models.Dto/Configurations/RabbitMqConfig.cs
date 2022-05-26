using LT.DigitalOffice.Kernel.BrokerSupport.Attributes;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.File;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Image;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Requests.Image;

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

    // positions

    [AutoInjectRequest(typeof(IGetPositionsRequest))]
    public string GetPositionsEndpoint { get; set; }

    // companies

    [AutoInjectRequest(typeof(IGetCompaniesRequest))]
    public string GetCompaniesEndpoint { get; set; }

    // image

    [AutoInjectRequest(typeof(IRemoveImagesPublish))]
    public string RemoveImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetImagesRequest))]
    public string GetImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateImagesPublish))]
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

    // file

    [AutoInjectRequest(typeof(ICreateFilesPublish))]
    public string CreateFilesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IRemoveFilesPublish))]
    public string RemoveFilesEndpoint { get; set; }
  }
}
