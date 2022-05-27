using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Command;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using MicroRabbit.Domain.Core.Bus;

namespace MicroRabbit.Banking.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRespository _accountRespository;
        private readonly IEventBus _bus;

        public AccountService(IAccountRespository accountRespository, IEventBus bus)
        {
            this._accountRespository = accountRespository;
            _bus = bus;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _accountRespository.GetAccounts();   
        }

        public void Transfer(AccountTransfer accountTranfer)
        {
            //Cria o objeto Command para enviar  a mensagem
            var createTransferCommand = new CreateTransferCommand(
                accountTranfer.FromAccount,
                accountTranfer.ToAccount,
                accountTranfer.TransferAmount
                );

            //Envia a mensagem
            _bus.SendCommand(createTransferCommand);    
        }
    }
}
