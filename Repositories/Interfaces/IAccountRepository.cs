using BankOfDotNet.Models;

namespace BankofDotNet.Repository.Interface;

public interface IAccountRepository
{
    Task<Account> CreateAsync(Account account);
    Task<bool> DeleteAsync(Guid accountId);
    Task<Account> GetAccountByIdAsync(Guid accountId);
    Task<IEnumerable<Account>> GetUserAccountsAsync(Guid accountId);
    Task<bool> UpdateAsync(Account account);
}
