using BankOfDotNet.Models;

namespace BankofDotNet.Repository.Interface;

public interface IBankTransactionRepository
{
    public Task<BankTransaction> AddBankTransactionAsync(BankTransaction transaction);
    public Task<BankTransaction> FindBankTransactionByIdAsync(Guid transactionId);
    public Task<IEnumerable<BankTransaction>> GetBankTransactionsByAccountIdAsync(Guid accountId);
    public Task<Account> FindAccountByIdAsync(Guid accountId);
    public Task UpdateAccountBalanceAsync(Guid accountId, decimal balance);
}
