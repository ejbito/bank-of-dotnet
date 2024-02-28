using BankofDotNet.DTOs.Account;

namespace BankofDotNet.Services.Interfaces;

public interface IAccountService
{
    Task<AccountReadDto> CreateAccountAsync(Guid userId, AccountCreateDto accountCreateDto);
    Task<IEnumerable<AccountReadDto>> GetUserAccountsAsync(Guid userId);
    Task<AccountReadDto> GetAccountByIdAsync(Guid accountId);
    Task<bool> UpdateAcountAsync(Guid accountId, Guid userId, AccountUpdateDto accountUpdateDto);
    Task<bool> UpdateAccountPinAsync(Guid accountId, Guid userId, AccountUpdatePinDto accountPinDto);
    Task<bool> DeleteAccountAsync(Guid accountId, Guid userId);
}
