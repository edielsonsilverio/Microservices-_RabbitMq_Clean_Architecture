namespace MicroRabbit.Banking.Domain.Command
{
    public class CreateTransferCommand : TransferCommand
    {
        public CreateTransferCommand(int from, int to, decimal amount)
        {
            From = from;    
            To = to;    
            Amount = amount; 
        }
    }
}

