using BankOfDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace BankOfDotNet.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Account { get; set; }
    public DbSet<BankTransaction> BankTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>()
            .HasMany(u => u.Accounts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        // Account
        modelBuilder.Entity<Account>()
            .Property(a => a.BSB)
            .HasMaxLength(6)
            .IsFixedLength();

        modelBuilder.Entity<Account>()
            .Property(a => a.ACC)
            .HasMaxLength(9)
            .IsFixedLength();

        modelBuilder.Entity<Account>()
            .HasIndex(a => new { a.BSB, a.ACC })
            .IsUnique();

        // BankTransaction
        modelBuilder.Entity<BankTransaction>()
            .HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BankTransaction>()
            .HasOne(t => t.FromAccount)
            .WithMany()
            .HasForeignKey(t => t.FromAccountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BankTransaction>()
            .HasOne(t => t.ToAccount)
            .WithMany()
            .HasForeignKey(t => t.ToAccountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BankTransaction>()
            .HasCheckConstraint("CK_BankTransaction_FromToAccount", "FromAccountId != ToAccountId");
    }
}
