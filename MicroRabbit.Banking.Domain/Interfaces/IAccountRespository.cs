using MicroRabbit.Banking.Domain.Models;

namespace MicroRabbit.Banking.Domain.Interfaces
{
    public interface IAccountRespository
    {
        IEnumerable<Account> GetAccounts();
    }
}
