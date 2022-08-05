using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.File;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using LT.DigitalOffice.ProjectService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker.Consumers
{
  public class CreateFilesConsumer : IConsumer<ICreateFilesPublish>
  {
    private readonly IProjectFileRepository _repository;
    private readonly IDbProjectFileMapper _mapper;

    public CreateFilesConsumer(
      IProjectFileRepository repository,
      IDbProjectFileMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<ICreateFilesPublish> context)
    {
      if (context.Message.FilesIds is not null && context.Message.FilesIds.Any())
      {
        await _repository.CreateAsync(context.Message.FilesIds
          .Select(x => _mapper.Map(x, context.Message.ProjectId, context.Message.Access)).ToList());
      }
    }
  }
}
