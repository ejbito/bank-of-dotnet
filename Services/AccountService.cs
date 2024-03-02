using BankofDotNet.DTOs.Account;
using BankofDotNet.DTOs.BankTransaction;
using BankofDotNet.Repository;
using BankofDotNet.Repository.Interface;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Data;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;


namespace BankofDotNet.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private Random _random = new Random();
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository accountRepository, IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<AccountReadDto> CreateAsync(Guid userId, AccountCreateDto dto)
    {
        _logger.LogInformation("Creating account for User ID: {UserId}", userId);

        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            throw new KeyNotFoundException($"User not found: {userId}");
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

        _logger.LogInformation("Account created: {AccountId} for User ID: {UserId}", newAccount.AccountId, userId);
        return ReadDto(newAccount);
    }

    public async Task<bool> DeleteAsync(Guid accountId, Guid userId)
    {
        _logger.LogInformation("Attempting to delete account: {AccountId} for User ID: {UserId}", accountId, userId);

        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            _logger.LogWarning("Attempted to delete a non-existent account: {AccountId}", accountId);
            throw new KeyNotFoundException($"Account not found: {accountId}");
        }

        if (account.UserId != userId)
        {
            _logger.LogWarning("User ID: {UserId} does not have permission to delete account: {AccountId}", userId, accountId);
            throw new UnauthorizedAccessException("You do not have permission to delete this account.");
        }

        await _accountRepository.DeleteAsync(accountId);

        _logger.LogInformation("Account deleted: {AccountId}", accountId);
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
        _logger.LogInformation("Retrieving account by ID: {AccountId}", accountId);

        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            _logger.LogWarning("Account not found: {AccountId}", accountId);
            throw new KeyNotFoundException($"Account not found: {accountId}");
        }

        return ReadDto(account);
    }

    public async Task<IEnumerable<AccountReadDto>> GetAllByUserIdAsync(Guid userId)
    {
        _logger.LogInformation("Retrieving all accounts for User ID: {UserId}", userId);

        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId} while attempting to retrieve accounts", userId);
            throw new KeyNotFoundException($"User not found: {userId}");
        }

        var accounts = await _accountRepository.GetUserAccountsAsync(userId);
        return accounts.Select(account => ReadDto(account)).ToList();
    }

    public async Task<bool> UpdatePinAsync(Guid accountId, Guid userId, AccountUpdatePinDto accountPinDto)
    {
        _logger.LogInformation("Attempting to update PIN for account: {AccountId}", accountId);

        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            _logger.LogWarning("Account not found: {AccountId} while attempting to update PIN", accountId);
            throw new KeyNotFoundException($"Account not found: {accountId}");
        }

        if (account.UserId != userId)
        {
            _logger.LogWarning("User ID: {UserId} does not have permission to update PIN for account: {AccountId}", userId, accountId);
            throw new UnauthorizedAccessException("You do not have permission to update this account's PIN.");
        }

        var currentPinAsString = accountPinDto.CurrentPin.ToString();
        var newPinAsString = accountPinDto.NewPin.ToString();
        var verificationResult = _passwordHasher.VerifyHashedPassword(null, account.Pin, currentPinAsString);
        
        if (verificationResult != PasswordVerificationResult.Success)
        {
            _logger.LogWarning("Incorrect current PIN provided for account: {AccountId}", accountId);
            throw new ArgumentException("Current PIN is incorrect.");
        }

        account.Pin = _passwordHasher.HashPassword(null, newPinAsString);
        await _accountRepository.UpdateAsync(account);

        _logger.LogInformation("PIN updated for account: {AccountId}", accountId);
        return true;
    }

    public async Task<bool> UpdateAsync(Guid accountId, Guid userId, AccountUpdateDto dto)
    {
        _logger.LogInformation("Attempting to update account: {AccountId}", accountId);

        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            _logger.LogWarning("Account not found: {AccountId} while attempting to update", accountId);
            throw new KeyNotFoundException($"Account not found: {accountId}");
        }

        if (account.UserId != userId)
        {
            _logger.LogWarning("User ID: {UserId} does not have permission to update account: {AccountId}", userId, accountId);
            throw new UnauthorizedAccessException("You do not have permission to update this account.");
        }

        account.AccountType = dto.AccountType;
        await _accountRepository.UpdateAsync(account);

        _logger.LogInformation("Account updated: {AccountId}", accountId);
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
