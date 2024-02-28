using BankofDotNet.DTOs.Account;
using BankofDotNet.Repository;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Data;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace BankofDotNet.Services;

public class AccountService : IAccountService
{
    private readonly AccountRepository _accountRepository;
    private readonly UserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountService(AccountRepository accountRepository, UserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AccountReadDto> CreateAccountAsync(Guid userId, AccountCreateDto accountCreateDto)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException();
        }
        var newAccount = new Account
        {
            UserId = userId,
            AccountType = accountCreateDto.AccountType,
            Balance = 0,
            Pin = _passwordHasher.HashPassword(null, accountCreateDto.Pin.ToString()),
            CreatedAt = DateTime.UtcNow
        };
        await _accountRepository.CreateAsync(newAccount);
        return new AccountReadDto
        {
            AccountId = newAccount.AccountId,
            UserId = newAccount.UserId,
            AccountType = newAccount.AccountType,
            Balance = newAccount.Balance,
        };
    }

    public async Task<bool> DeleteAccountAsync(Guid accountId, Guid userId)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException();
        }
        if (account.UserId == userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this account.");
        }
        await _accountRepository.DeleteAsync(accountId);
        return true;
    }

    public async Task<AccountReadDto> GetAccountByIdAsync(Guid accountId)
    {
        var account = await _accountRepository.GetAccountByIdAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException("Account not found");
        }
        return account;
    }

    public async Task<IEnumerable<AccountReadDto>> GetUserAccountsAsync(Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        var accounts = await _accountRepository.GetUserAccountsAsync(userId);
        return accounts;
    }

    public async Task<bool> UpdateAccountPinAsync(Guid accountId, Guid userId, AccountUpdatePinDto accountPinDto)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException();
        }
        if (account.UserId == userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this account.");
        }
        var currentPinAsString = accountPinDto.CurrentPin.ToString();
        var newPinAsString = accountPinDto.NewPin.ToString();
        var verificationResult = _passwordHasher.VerifyHashedPassword(null, account.Pin, currentPinAsString);
        if (verificationResult != PasswordVerificationResult.Success)
        {
            throw new ArgumentException("Current PIN is incorrect.");
        }
        account.Pin = _passwordHasher.HashPassword(null, newPinAsString);
        await _accountRepository.UpdateAsync(account);
        return true;
    }

    public async Task<bool> UpdateAcountAsync(Guid accountId, Guid userId, AccountUpdateDto accountUpdateDto)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException();
        }
        if (account.UserId == userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this account.");
        }
        account.AccountType = accountUpdateDto.AccountType;
        await _accountRepository.UpdateAsync(account);
        return true;
    }
}
