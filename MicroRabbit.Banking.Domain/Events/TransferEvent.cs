using MicroRabbit.Domain.Core.Events;

namespace MicroRabbit.Banking.Domain.Events
{
    public abstract class TransferEvent : Event
    {
        public int From { get; protected set; }
        public int To { get; protected set; }
        public decimal Amount { get; set; }
    }
}
