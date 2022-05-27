namespace MicroRabbit.Banking.Domain.Command
{
    public abstract class TransferCommand : MicroRabbit.Domain.Core.Commands.Command
    {
        public int From { get; protected set; }
        public int To { get; protected set; }
        public decimal Amount { get; set; }
    }
}

