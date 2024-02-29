using BankofDotNet.DTOs.Account;
using BankOfDotNet.Models;

namespace BankofDotNet.Repository.Interface;

public interface IAccountRepository
{
    Task<Account> CreateAsync(Account account);
    Task<bool> DeleteAsync(Guid accountId);
    Task<Account> FindByIdAsync(Guid accountId);
    Task<AccountReadDto> GetAccountByIdAsync(Guid accountId);
    Task<IEnumerable<AccountReadDto>> GetUserAccountsAsync(Guid accountId);
    Task<bool> UpdateAsync(Account account);
    Task<bool> IsUniqueBSBAndACC(string bsb, string acc);
}
