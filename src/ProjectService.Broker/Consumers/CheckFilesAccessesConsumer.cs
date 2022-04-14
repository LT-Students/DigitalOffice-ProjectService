using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Models.Db;
using LT.DigitalOffice.ProjectService.Models.Dto.Enums;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Consumers
{
  public class CheckFilesAccessesConsumer : IConsumer<ICheckProjectFilesAccessesRequest>
  {
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IAccessValidator _accessValidator;

    public CheckFilesAccessesConsumer(
      IUserRepository userRepository,
      IFileRepository fileRepository,
      IAccessValidator accessValidator)
    {
      _userRepository = userRepository;
      _fileRepository = fileRepository;
      _accessValidator = accessValidator;
    }

    public async Task Consume(ConsumeContext<ICheckProjectFilesAccessesRequest> context)
    {
      AccessType accessType = AccessType.Public;
      Guid userId = context.Message.UserId;
      bool isManage = false;

      if (await _accessValidator.HasRightsAsync(userId, Rights.AddEditRemoveProjects))
      {
        isManage = true;
      } 

      List<DbProjectFile> files = await _fileRepository.GetAsync(context.Message.FilesIds);
      List<Guid> resultFiles = null;

      foreach (DbProjectFile file in files)
      {
        DbProjectUser dbProjectUser = (await _userRepository.GetAsync(new List<Guid>() { userId }, file.ProjectId))
          ?.FirstOrDefault();

        if (isManage || dbProjectUser?.Role == (int)ProjectUserRoleType.Manager)
        {
          accessType = AccessType.Manager;
        }
        else if (dbProjectUser is not null)
        {
          accessType = AccessType.Team;
        }

        if (file.Access == (int)accessType)
        {
          resultFiles.Add(file.FileId);
        }
      }

      object response = OperationResultWrapper.CreateResponse((_) => resultFiles, context);

      await context.RespondAsync<IOperationResult<List<Guid>>>(response);
    }
  }
}
