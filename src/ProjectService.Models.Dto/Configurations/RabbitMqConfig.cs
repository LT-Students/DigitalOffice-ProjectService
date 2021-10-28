using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Time;
using LT.DigitalOffice.Models.Broker.Requests.User;
using IGetImagesRequest = LT.DigitalOffice.Models.Broker.Requests.Image.IGetImagesRequest;

namespace LT.DigitalOffice.ProjectService.Models.Dto.Configurations
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    public string SearchProjectsEndpoint { get; set; }
    public string FindParseEntitiesEndpoint { get; set; }
    public string GetProjectsUsersEndpoint { get; set; }
    public string GetProjectsEndpoint { get; set; }
    public string DisactivateUserEndpoint { get; set; }
    public string CheckProjectsExistenceEndpoint { get; set; }
    public string CheckProjectUsersExistenceEndpoint { get; set; }

    // positions

    [AutoInjectRequest(typeof(IGetPositionsRequest))]
    public string GetPositionsEndpoint { get; set; }

    // image

    [AutoInjectRequest(typeof(IRemoveImagesRequest))]
    public string RemoveImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetImagesRequest))]
    public string GetImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateImagesRequest))]
    public string CreateImagesEndpoint { get; set; }

    // department

    [AutoInjectRequest(typeof(IGetDepartmentsRequest))]
    public string GetDepartmentsEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICreateDepartmentEntityRequest))]
    public string CreateDepartmentEntityEndpoint {get; set;}

    // company

    [AutoInjectRequest(typeof(IGetOfficesRequest))]
    public string GetOfficesEndpoint { get; set; }

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
