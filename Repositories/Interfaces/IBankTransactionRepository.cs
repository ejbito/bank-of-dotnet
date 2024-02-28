using BankOfDotNet.Models;

namespace BankofDotNet.Repository.Interface;

public interface IBankTransactionRepository
{
    Task<BankTransaction> AddTransactionAsync(BankTransaction transaction);
    Task<BankTransaction> FindBankTransactionByIdAsync(Guid transactionId);
    Task<IEnumerable<BankTransaction>> GetBankTransactionsByAccountIdAsync(Guid accountId);
    Task<Account> FindAccountByIdAsync(Guid accountId);
    Task UpdateAccountBalanceAsync(Guid accountId, decimal balance);
}
