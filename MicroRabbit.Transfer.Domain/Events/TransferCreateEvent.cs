using MicroRabbit.Domain.Core.Events;

namespace MicroRabbit.Transfer.Domain.Events
{
    public class TransferCreateEvent : Event
    {
        public int From { get; protected set; }
        public int To { get; protected set; }
        public decimal Amount { get; set; }

        public TransferCreateEvent(int from, int to, decimal amount)
        {
            From = from;
            To = to;
            Amount = amount;
        }
    }
}
