using BankOfDotNet.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankOfDotNet.Models;

public class Account
{
    [Key]
    public Guid AccountId { get; set; } = Guid.NewGuid();
    public string BSB { get; set; }
    public string ACC { get; set; }
    public AccountType AccountType { get; set; }
    public string Pin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; }

    public virtual ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
}
