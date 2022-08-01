using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Models.File;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.ProjectService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.ProjectService.Broker.Requests
{
  public class FileService : IFileService
  {
    private readonly ILogger<FileService> _logger;
    private readonly IRequestClient<IGetFilesRequest> _rcGetFiles;

    public FileService(
      ILogger<FileService> logger,
      IRequestClient<IGetFilesRequest> rcGetFiles)
    {
      _logger = logger;
      _rcGetFiles = rcGetFiles;
    }

    public async Task<List<FileCharacteristicsData>> GetFilesCharacteristicsAsync(List<Guid> filesIds, List<string> errors = null)
    {
      if (filesIds is null || !filesIds.Any())
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<IGetFilesRequest, IGetFilesResponse>(
          _rcGetFiles,
          IGetFilesRequest.CreateObj(filesIds),
          errors,
          _logger))
        ?.FilesCharacteristicsData;
    }
  }
}
