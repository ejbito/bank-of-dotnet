using BankOfDotNet.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankOfDotNet.Models;

public class BankTransaction
{
    [Key]
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public BankTransactionType BankTransactionType { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? FromAccountId { get; set; }
    public Guid? ToAccountId { get; set; }
    public Guid AccountId { get; set; }

    public virtual Account Account { get; set; }
    public virtual Account FromAccount { get; set; }
    public virtual Account ToAccount { get; set; }
}
