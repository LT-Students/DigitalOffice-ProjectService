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

      DbProjectUser dbProjectUser = (await _userRepository.GetAsync(new List<Guid>() { context.Message.UserId }))
        ?.FirstOrDefault();

      if (await _accessValidator.HasRightsAsync(Rights.AddEditRemoveProjects)
        || dbProjectUser?.Role == (int)ProjectUserRoleType.Manager)
      {
        accessType = AccessType.Manager;
      }
      else if (dbProjectUser is not null)
      {
        accessType = AccessType.Team;
      }

      List<DbProjectFile> files =  await _fileRepository.GetAsync(context.Message.FilesIds);

      object response = OperationResultWrapper.CreateResponse(
        (_) => files.Select(x => x.Access >= (int)accessType), context);

      await context.RespondAsync<IOperationResult<List<Guid>>>(response);
    }
  }
}
