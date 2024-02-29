using BankofDotNet.DTOs.Account;
using BankofDotNet.DTOs.BankTransaction;
using BankofDotNet.Repository;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Data;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;


namespace BankofDotNet.Services;

public class AccountService : IAccountService
{
    private readonly AccountRepository _accountRepository;
    private readonly UserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private Random _random = new Random();

    public AccountService(AccountRepository accountRepository, UserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AccountReadDto> CreateAsync(Guid userId, AccountCreateDto dto)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        var (bsb, acc) = await GenerateUniqueBSBAndACCAsync();

        var newAccount = new Account
        {
            UserId = userId,
            AccountType = dto.AccountType,
            BSB = bsb,
            ACC = acc,
            Balance = 0,
            Pin = _passwordHasher.HashPassword(null, dto.Pin.ToString()),
            CreatedAt = DateTime.UtcNow
        };
        await _accountRepository.CreateAsync(newAccount);
        return new AccountReadDto
        {
            AccountId = newAccount.AccountId,
            UserId = newAccount.UserId,
            BSB = newAccount.BSB,
            ACC = newAccount.ACC,
            AccountType = newAccount.AccountType,
            Balance = newAccount.Balance,
        };
    }

    public async Task<bool> DeleteAsync(Guid accountId, Guid userId)
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

    public async Task<(string BSB, string ACC)> GenerateUniqueBSBAndACCAsync()
    {
        string bsb, acc;
        do
        {
            bsb = GenerateRandomNumber(6);
            acc = GenerateRandomNumber(9);
        }
        while (!await _accountRepository.IsUniqueBSBAndACC(bsb, acc));

        return (bsb, acc);
    }

    public async Task<AccountReadDto> GetByIdAsync(Guid accountId)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException("Account not found");
        }
        return ReadDto(account);
    }

    public async Task<IEnumerable<AccountReadDto>> GetAllByUserIdAsync(Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        var accounts = await _accountRepository.GetUserAccountsAsync(userId);
        return accounts.Select(account => ReadDto(account)).ToList();
    }

    public async Task<bool> UpdatePinAsync(Guid accountId, Guid userId, AccountUpdatePinDto accountPinDto)
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

    public async Task<bool> UpdateAsync(Guid accountId, Guid userId, AccountUpdateDto dto)
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
        account.AccountType = dto.AccountType;
        await _accountRepository.UpdateAsync(account);
        return true;
    }

    private string GenerateRandomNumber(int length)
    {
        var chars = Enumerable.Range(0, length).Select(_ => (char)('0' + _random.Next(0, 10))).ToArray();
        return new string(chars);
    }

    private static AccountReadDto ReadDto(Account account)
    {
        return new AccountReadDto
        {
            AccountId = account.AccountId,
            UserId = account.UserId,
            BSB = account.BSB,
            ACC = account.ACC,
            AccountType = account.AccountType,
            Balance = account.Balance,
        };
    }
}
