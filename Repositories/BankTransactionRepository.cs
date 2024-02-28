using BankofDotNet.Repository.Interface;
using BankOfDotNet.Data;
using BankOfDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace BankofDotNet.Repository;

public class BankTransactionRepository : IBankTransactionRepository
{
    private readonly AppDbContext _context;

    public BankTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BankTransaction> AddTransactionAsync(BankTransaction transaction)
    {
        _context.BankTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Account> FindAccountByIdAsync(Guid accountId)
    {
        return await _context.Accounts.FindAsync(accountId);
    }

    public async Task<BankTransaction> FindBankTransactionByIdAsync(Guid transactionId)
    {
        return await _context.BankTransactions.FindAsync(transactionId);
    }

    public async Task<IEnumerable<BankTransaction>> GetBankTransactionsByAccountIdAsync(Guid accountId)
    {
        return await _context.BankTransactions
            .Where(t => t.AccountId == accountId || t.FromAccountId == accountId || t.ToAccountId == accountId)
            .ToListAsync();
    }

    public async Task UpdateAccountBalanceAsync(Guid accountId, decimal newBalance)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account != null)
        {
            account.Balance = newBalance;
            await _context.SaveChangesAsync();
        }
    }
}
