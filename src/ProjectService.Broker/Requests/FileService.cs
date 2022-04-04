using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class FileService : IFileService
  {
    private readonly ILogger<FileService> _logger;
    private readonly IRequestClient<ICreateFilesRequest> _rcCreateFiles;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileService(
      ILogger<FileService> logger,
      IRequestClient<ICreateFilesRequest> rcCreateFiles,
      IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _rcCreateFiles = rcCreateFiles;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> CreateFilesAsync(List<FileData> files, List<string> errors)
    {
      return files is null || !files.Any()
        ? false
        : (await RequestHandler
          .ProcessRequest<ICreateFilesRequest, bool>(
            _rcCreateFiles,
            ICreateFilesRequest.CreateObj(files,
              _httpContextAccessor.HttpContext.GetUserId()),
            errors,
            _logger));
    }
  }
}
