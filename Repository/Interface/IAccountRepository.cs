using BankOfDotNet.Models;

namespace BankofDotNet.Repository.Interface;

public interface IAccountRepository
{
    public Task<Account> CreateAsync(Account account);
    public Task<bool> DeleteAsync(Guid accountId);
    public Task<Account> GetAccountByIdAsync(Guid accountId);
    public Task<IEnumerable<Account>> GetUserAccountsAsync(Guid accountId);
    public Task<bool> UpdateAsync(Account account);
}
