using BankOfDotNet.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankofDotNet.DTOs.BankTransaction;

public class BankTransactionReadDto
{
    public Guid TransactionId { get; set; }

    public Guid AccountId { get; set; }

    public BankTransactionType BankTransactionType { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? FromAccountId { get; set; }

    public Guid? ToAccountId { get; set; }
}
