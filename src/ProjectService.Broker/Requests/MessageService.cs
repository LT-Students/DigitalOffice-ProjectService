using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class MessageService : IMessageService
  {
    private readonly IRequestClient<ICreateWorkspaceRequest> _rcCreateWorkspace;
    private readonly ILogger<MessageService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;

    public MessageService(
      IRequestClient<ICreateWorkspaceRequest> rcCreateWorkspace,
      ILogger<MessageService> logger,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _rcCreateWorkspace = rcCreateWorkspace;
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task CreateWorkspaceAsync(string projectName, List<Guid> usersIds, List<string> errors)
    {
      await RequestHandler.ProcessRequest<ICreateWorkspaceRequest, bool>(
        _rcCreateWorkspace,
        ICreateWorkspaceRequest.CreateObj(
          projectName,
          _httpContextAccessor.HttpContext.GetUserId(),
          usersIds),
        errors,
        _logger);
    }
  }
}
