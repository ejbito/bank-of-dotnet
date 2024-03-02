using BankofDotNet.DTOs.Account;
using BankofDotNet.Repository.Interface;
using BankOfDotNet.Data;
using BankOfDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace BankofDotNet.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account> CreateAsync(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account), "Account to create cannot be null.");
        }

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> DeleteAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            return false;
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Account> FindByIdAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException($"Account with ID {accountId} not found.");
        }
        return account;
    }

    public async Task<bool> IsUniqueBSBAndACC(string bsb, string acc)
    {
        if (string.IsNullOrWhiteSpace(bsb)) throw new ArgumentException("BSB cannot be null or whitespace.", nameof(bsb));
        if (string.IsNullOrWhiteSpace(acc)) throw new ArgumentException("ACC cannot be null or whitespace.", nameof(acc));

        return !await _context.Accounts.AnyAsync(a => a.BSB == bsb && a.ACC == acc);
    }

    public async Task<IEnumerable<Account>> GetUserAccountsAsync(Guid userId)
    {
        return await _context.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account), "Account to update cannot be null.");
        }

        var existingAccount = await _context.Accounts.FindAsync(account.AccountId);
        if (existingAccount == null)
        {
            throw new KeyNotFoundException($"Account with ID {account.AccountId} not found for update.");
        }

        _context.Entry(existingAccount).CurrentValues.SetValues(account);
        await _context.SaveChangesAsync();
        return true;
    }
}
