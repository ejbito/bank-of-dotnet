using BankofDotNet.DTOs.BankTransaction;
using BankofDotNet.Repository;
using BankofDotNet.Repository.Interface;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Enums;
using BankOfDotNet.Models;
using Microsoft.Identity.Client;
using System.Security.Principal;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BankofDotNet.Services;

public class BankTransactionService : IBankTransactionService
{
    private readonly IBankTransactionRepository _bankTransactionRepository;
    private readonly IAccountRepository _accountRepository;

    public BankTransactionService(IBankTransactionRepository bankTransactionRepository, IAccountRepository accountRepository)
    {
        _bankTransactionRepository = bankTransactionRepository;
        _accountRepository = accountRepository;
    }

    public async Task<BankTransactionReadDto> DepositAsync(Guid accountId, decimal amount)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException("Account not found.");
        }
        account.Balance += amount;
        await _bankTransactionRepository.UpdateBalanceAsync(accountId, account.Balance);
        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            BankTransactionType = BankTransactionType.Deposit,
            CreatedAt = DateTime.UtcNow,
        };
        await _bankTransactionRepository.CreateAsync(transaction);
        return ReadDto(transaction);
    }

    public async Task<IEnumerable<BankTransactionReadDto>> GetTransactionsByAccountAsync(Guid accountId)
    {
        var transactions = await _bankTransactionRepository.FindByAccountIdAsync(accountId);
        if (transactions == null)
        {
            throw new KeyNotFoundException();
        }
        return transactions.Select(t => ReadDto(t)).ToList();
    }

    public async Task<BankTransactionReadDto> GetTransactionByIdAsync(Guid transactionId)
    {
        var transaction = await _bankTransactionRepository.FindByTransactionIdAsync(transactionId);
        if (transaction == null)
        {
            throw new KeyNotFoundException();
        }
        return ReadDto(transaction);
    }

    public async Task<BankTransactionReadDto> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        var fromAccount = await _accountRepository.FindByIdAsync(fromAccountId);
        var toAccount = await _accountRepository.FindByIdAsync(toAccountId);
        if (fromAccount == null || toAccount == null)
        {
            throw new KeyNotFoundException("Account not found.");
        }
        if (fromAccount.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }
        fromAccount.Balance -= amount;
        toAccount.Balance += amount;
        await _bankTransactionRepository.UpdateBalanceAsync(fromAccountId, fromAccount.Balance);
        await _bankTransactionRepository.UpdateBalanceAsync(toAccountId, toAccount.Balance);
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
        return ReadDto(transaction);
    }

    public async Task<BankTransactionReadDto> WithdrawAsync(Guid accountId, decimal amount)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException("Account not found.");
        }
        if (account.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }
        account.Balance -= amount;
        await _bankTransactionRepository.UpdateBalanceAsync(accountId, account.Balance);
        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            BankTransactionType = BankTransactionType.Deposit,
            CreatedAt = DateTime.UtcNow,
        };
        await _bankTransactionRepository.CreateAsync(transaction);
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
