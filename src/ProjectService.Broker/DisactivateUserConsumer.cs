using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.ProjectService.Broker
{
  public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IUserRepository _repository;

    public DisactivateUserConsumer(
      IUserRepository repository)
    {
      _repository = repository;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      await _repository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy);
    }
  }
}
