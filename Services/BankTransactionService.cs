using BankofDotNet.DTOs.BankTransaction;
using BankofDotNet.Repository;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Enums;
using BankOfDotNet.Models;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BankofDotNet.Services;

public class BankTransactionService : IBankTransactionService
{
    private readonly BankTransactionRepository _bankTransactionRepository;
    private readonly AccountRepository _accountRepository;

    public BankTransactionService(BankTransactionRepository bankTransactionRepository, AccountRepository accountRepository)
    {
        _bankTransactionRepository = bankTransactionRepository;
        _accountRepository = accountRepository;
    }

    public async Task<BankTransactionReadDto> DepositAsync(Guid accountId, decimal amount)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            BankTransactionType = BankTransactionType.Deposit,
            CreatedAt = DateTime.UtcNow,
        };
        await _accountRepository.CreateAsync(account);
        return ReadDto(transaction);
    }

    public async Task<IEnumerable<BankTransactionReadDto>> GetBankTransactionByAccountId(Guid accountId)
    {
        var transactions = await _bankTransactionRepository.GetBankTransactionsByAccountIdAsync(accountId);
        if (transactions == null)
        {
            throw new KeyNotFoundException();
        }
        return transactions.Select(t => ReadDto(t)).ToList();
    }

    public async Task<BankTransactionReadDto> GetBankTransactionByTransactionId(Guid transactionId)
    {
        var transaction = await _bankTransactionRepository.FindBankTransactionByIdAsync(transactionId);
        if (transaction == null)
        {
            throw new KeyNotFoundException();
        }
        return ReadDto(transaction);
    }

    public Task<BankTransactionReadDto> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        throw new NotImplementedException();
    }

    public async Task<BankTransactionReadDto> WithdrawAsync(Guid accountId, decimal amount)
    {
        var account = await _accountRepository.FindByIdAsync(accountId);
        var transaction = new BankTransaction
        {
            AccountId = accountId,
            Amount = amount,
            BankTransactionType = BankTransactionType.Deposit,
            CreatedAt = DateTime.UtcNow,
        };
        await _accountRepository.CreateAsync(account);
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
