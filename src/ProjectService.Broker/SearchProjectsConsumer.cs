using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Search;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class SearchProjectsConsumer : IConsumer<ISearchProjectsRequest>
  {
    private IProjectRepository _projectRepository;

    private async Task<object> SearchProjectsAsync(string text)
    {
      List<DbProject> projects = await _projectRepository.SearchAsync(text);

      return ISearchResponse.CreateObj(
        projects.Select(
          p => new SearchInfo(p.Id, p.Name)).ToList());
    }

    public SearchProjectsConsumer(
      IProjectRepository projectRepository)
    {
      _projectRepository = projectRepository;
    }

    public async Task Consume(ConsumeContext<ISearchProjectsRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(SearchProjectsAsync, context.Message.Value);

      await context.RespondAsync<IOperationResult<ISearchResponse>>(response);
    }
  }
}
