using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Domain.EventHanlders
{
    public class TransferEventHandler : IEventHandler<TransferCreateEvent>
    {
        private readonly ITransferRepository _transferRepository;
        public TransferEventHandler(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }
        public Task Handle(TransferCreateEvent @event)
        {
            var transfer = new TransferLog
            {
                FromAccount = @event.From.ToString(),
                ToAccount = @event.To.ToString(),
                TransferAccount = @event.Amount
            };
            _transferRepository.AddTransferLog(transfer);

            return Task.CompletedTask;
        }
    }
}
