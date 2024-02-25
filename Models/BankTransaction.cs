using BankOfDotNet.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankOfDotNet.Models;

public class BankTransaction
{
    [Key]
    public Guid TransactionId { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public BankTransactionType BankTransactionType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? FromAccountId { get; set; }

    public Guid? ToAccountId { get; set; }

    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; }

    [ForeignKey("FromAccountId")]
    public virtual Account FromAccount { get; set; }

    [ForeignKey("ToAccountId")]
    public virtual Account ToAccount { get; set;}
}
