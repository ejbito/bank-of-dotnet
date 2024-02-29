using BankofDotNet.Repository.Interface;
using BankOfDotNet.Data;
using BankOfDotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BankofDotNet.Repository;

public class BankTransactionRepository : IBankTransactionRepository
{
    private readonly AppDbContext _context;

    public BankTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BankTransaction> CreateAsync(BankTransaction transaction)
    {
        _context.BankTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<BankTransaction> FindByTransactionIdAsync(Guid transactionId)
    {
        var transaction = await _context.BankTransactions.FindAsync(transactionId);
        if (transaction == null)
        {
            throw new KeyNotFoundException("Account not found.");
        }
        return transaction;
    }

    public async Task<IEnumerable<BankTransaction>> FindByAccountIdAsync(Guid accountId)
    {
        return await _context.BankTransactions
            .Where(t => t.AccountId == accountId || t.FromAccountId == accountId || t.ToAccountId == accountId)
            .ToListAsync();
    }

    public async Task UpdateBalanceAsync(Guid accountId, decimal newBalance)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account != null)
        {
            account.Balance = newBalance;
            await _context.SaveChangesAsync();
        }
    }
}
