using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.ProjectService.Data.Interfaces;
using MassTransit;
using System.Threading.Tasks;

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

        public Task Consume(ConsumeContext<IDisactivateUserRequest> context)
        {
            _repository.Remove(context.Message.UserId);

            return Task.FromResult(0);
        }
    }
}
