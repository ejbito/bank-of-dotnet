using BankofDotNet.DTOs.BankTransaction;
using BankofDotNet.Repository;
using BankofDotNet.Repository.Interface;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Enums;
using BankOfDotNet.Models;

namespace BankofDotNet.Services;

public class BankTransactionService : IBankTransactionService
{
    private readonly IBankTransactionRepository _bankTransactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<BankTransactionService> _logger;

    public BankTransactionService(IBankTransactionRepository bankTransactionRepository, IAccountRepository accountRepository, ILogger<BankTransactionService> logger)
    {
        _bankTransactionRepository = bankTransactionRepository;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<BankTransactionReadDto> DepositAsync(Guid accountId, decimal amount)
    {
        _logger.LogInformation($"Initiating deposit for Account ID: {accountId}, Amount: {amount}");

        if (amount <= 0)
        {
            _logger.LogWarning($"Invalid deposit amount: {amount}. Amount must be positive.");
            throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
        }

        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            _logger.LogError($"Account not found: {accountId}. Deposit failed.");
            throw new KeyNotFoundException($"Account not found: {accountId}.");
        }

        account.Balance += amount;
        await _accountRepository.UpdateAsync(account);

        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            BankTransactionType = BankTransactionType.Deposit,
            CreatedAt = DateTime.UtcNow,
        };

        await _bankTransactionRepository.CreateAsync(transaction);
        _logger.LogInformation($"Deposit successful for Account ID: {accountId}, Amount: {amount}");

        return ReadDto(transaction);
    }

    public async Task<IEnumerable<BankTransactionReadDto>> GetTransactionsByAccountAsync(Guid accountId)
    {
        _logger.LogInformation($"Retrieving transactions for Account ID: {accountId}");

        var transactions = await _bankTransactionRepository.FindByAccountIdAsync(accountId);
        if (transactions == null || !transactions.Any())
        {
            _logger.LogWarning($"No transactions found for Account ID: {accountId}");
            throw new KeyNotFoundException($"No transactions found for Account ID: {accountId}.");
        }

        return transactions.Select(t => ReadDto(t)).ToList();
    }

    public async Task<BankTransactionReadDto> GetTransactionByIdAsync(Guid transactionId)
    {
        _logger.LogInformation($"Retrieving transaction by ID: {transactionId}");

        var transaction = await _bankTransactionRepository.FindByTransactionIdAsync(transactionId);
        if (transaction == null)
        {
            _logger.LogError($"Transaction not found: {transactionId}");
            throw new KeyNotFoundException($"Transaction not found: {transactionId}.");
        }

        return ReadDto(transaction);
    }

    public async Task<BankTransactionReadDto> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        _logger.LogInformation($"Initiating transfer from Account ID: {fromAccountId} to Account ID: {toAccountId}, Amount: {amount}");

        if (amount <= 0)
        {
            _logger.LogWarning($"Invalid transfer amount: {amount}. Amount must be positive.");
            throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));
        }

        var fromAccount = await _accountRepository.FindByIdAsync(fromAccountId);
        var toAccount = await _accountRepository.FindByIdAsync(toAccountId);
        if (fromAccount == null || toAccount == null)
        {
            _logger.LogError($"Accounts for transfer not found. From Account ID: {fromAccountId}, To Account ID: {toAccountId}");
            throw new KeyNotFoundException("One or both accounts not found.");
        }

        if (fromAccount.Balance < amount)
        {
            _logger.LogWarning($"Insufficient funds for transfer. From Account ID: {fromAccountId}, Available Balance: {fromAccount.Balance}, Transfer Amount: {amount}");
            throw new InvalidOperationException("Insufficient funds for transfer.");
        }

        fromAccount.Balance -= amount;
        toAccount.Balance += amount;
        await _accountRepository.UpdateAsync(fromAccount);
        await _accountRepository.UpdateAsync(toAccount);

        var transaction = new BankTransaction
        {
            AccountId = fromAccountId,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            BankTransactionType = BankTransactionType.Transfer,
            CreatedAt = DateTime.UtcNow,
        };

        await _bankTransactionRepository.CreateAsync(transaction);
        _logger.LogInformation($"Transfer successful from Account ID: {fromAccountId} to Account ID: {toAccountId}, Amount: {amount}");

        return ReadDto(transaction);
    }

    public async Task<BankTransactionReadDto> WithdrawAsync(Guid accountId, decimal amount)
    {
        _logger.LogInformation($"Initiating withdrawal for Account ID: {accountId}, Amount: {amount}");

        if (amount <= 0)
        {
            _logger.LogWarning($"Invalid withdrawal amount: {amount}. Amount must be positive.");
            throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));
        }

        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            _logger.LogError($"Account not found: {accountId}. Withdrawal failed.");
            throw new KeyNotFoundException($"Account not found: {accountId}.");
        }

        if (account.Balance < amount)
        {
            _logger.LogWarning($"Insufficient funds for withdrawal. Account ID: {accountId}, Available Balance: {account.Balance}, Withdrawal Amount: {amount}");
            throw new InvalidOperationException("Insufficient funds for withdrawal.");
        }

        account.Balance -= amount;
        await _accountRepository.UpdateAsync(account);

        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            BankTransactionType = BankTransactionType.Withdrawal,
            CreatedAt = DateTime.UtcNow,
        };

        await _bankTransactionRepository.CreateAsync(transaction);
        _logger.LogInformation($"Withdrawal successful for Account ID: {accountId}, Amount: {amount}");

        return ReadDto(transaction);
    }

    private static BankTransactionReadDto ReadDto(BankTransaction bankTransaction)
    {
        return new BankTransactionReadDto
        {
            TransactionId = bankTransaction.TransactionId,
            AccountId = bankTransaction.AccountId,
            BankTransactionType = bankTransaction.BankTransactionType,
            Amount = bankTransaction.Amount,
            CreatedAt = bankTransaction.CreatedAt,
            FromAccountId = bankTransaction.FromAccountId,
            ToAccountId = bankTransaction.ToAccountId,
        };
    }
}