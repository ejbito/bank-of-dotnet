using BankOfDotNet.Models;

namespace BankofDotNet.Repository.Interface;

public interface IBankTransactionRepository
{
    Task<BankTransaction> CreateAsync(BankTransaction transaction);
    Task<BankTransaction> FindByTransactionIdAsync(Guid transactionId);
    Task<IEnumerable<BankTransaction>> FindByAccountIdAsync(Guid accountId);
    Task UpdateBalanceAsync(Guid accountId, decimal balance);
}
