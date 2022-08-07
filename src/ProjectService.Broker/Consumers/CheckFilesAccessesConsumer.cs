using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Consumers
{
  public class CheckFilesAccessesConsumer : IConsumer<ICheckProjectFilesAccessesRequest>
  {
    private readonly IProjectUserRepository _userRepository;
    private readonly IProjectFileRepository _fileRepository;

    public CheckFilesAccessesConsumer(
      IProjectUserRepository userRepository,
      IProjectFileRepository fileRepository)
    {
      _userRepository = userRepository;
      _fileRepository = fileRepository;
    }

    public async Task Consume(ConsumeContext<ICheckProjectFilesAccessesRequest> context)
    {
      FileAccessType accessType = FileAccessType.Public;

      List<DbProjectFile> files = await _fileRepository.GetAsync(context.Message.FilesIds);
      List<Guid> resultFiles = new List<Guid>();
      List<DbProjectUser> dbProjectUsers = await _userRepository.GetAsync(new List<Guid>() { context.Message.UserId });

      foreach (DbProjectFile file in files)
      {
        DbProjectUser dbProjectUser = dbProjectUsers.Where(x => x.ProjectId == file.ProjectId).FirstOrDefault();

        if (dbProjectUser?.Role == (int)ProjectUserRoleType.Manager)
        {
          accessType = FileAccessType.Manager;
        }
        else if (dbProjectUser is not null)
        {
          accessType = FileAccessType.Team;
        }

        if (file.Access >= (int)accessType)
        {
          resultFiles.Add(file.FileId);
        }
      }

      object response = OperationResultWrapper.CreateResponse((_) => resultFiles, context);

      await context.RespondAsync<IOperationResult<List<Guid>>>(response);
    }
  }
}
