using BankofDotNet.DTOs.Account;

namespace BankofDotNet.Services.Interfaces;

public interface IAccountService
{
    Task<AccountReadDto> CreateAsync(Guid userId, AccountCreateDto dto);
    Task<IEnumerable<AccountReadDto>> GetAllByUserIdAsync(Guid userId);
    Task<AccountReadDto> GetByIdAsync(Guid accountId);
    Task<bool> UpdateAsync(Guid accountId, Guid userId, AccountUpdateDto dto);
    Task<bool> UpdatePinAsync(Guid accountId, Guid userId, AccountUpdatePinDto accountPinDto);
    Task<bool> DeleteAsync(Guid accountId, Guid userId);
    Task<(string BSB, string ACC)> GenerateUniqueBSBAndACCAsync();
}
