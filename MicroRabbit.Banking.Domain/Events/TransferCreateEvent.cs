namespace MicroRabbit.Banking.Domain.Events
{
    public class TransferCreateEvent : TransferEvent
    {
        public TransferCreateEvent(int from, int to, decimal amount)
        {
            From = from;
            To = to;
            Amount = amount;
        }
    }
}
