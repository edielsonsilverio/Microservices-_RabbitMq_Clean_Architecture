using MediatR;
using MicroRabbit.Banking.Domain.Command;
using MicroRabbit.Banking.Domain.Events;
using MicroRabbit.Domain.Core.Bus;

namespace MicroRabbit.Banking.Domain.CommandHandlers
{
    public class TransferCommandHandler : IRequestHandler<CreateTransferCommand, bool>
    {
        private readonly IEventBus _bus;

        public TransferCommandHandler(IEventBus bus)
        {
            _bus = bus;
        }

        public Task<bool> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
        {
            //Lógica para publicar a mensagem dentro do evento bus rabbitMq

            _bus.Publish(new TransferCreateEvent(request.From, request.To, request.Amount));

            return Task.FromResult(true);
        }
    }
}
