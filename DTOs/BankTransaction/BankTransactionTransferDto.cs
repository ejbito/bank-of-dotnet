using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankofDotNet.DTOs.BankTransaction;

public class BankTransactionTransferDto
{
    [Required]
    public Guid FromAccountId { get; set; }

    [Required]
    public Guid ToAccountId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }
}
