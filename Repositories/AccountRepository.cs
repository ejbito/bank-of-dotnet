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
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> DeleteAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<Account> FindByIdAsync(Guid accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            throw new KeyNotFoundException("Account not found.");
        }
        return account;
    }

    public async Task<bool> IsUniqueBSBAndACC(string bsb, string acc)
    {
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
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        return true;
    }
}
