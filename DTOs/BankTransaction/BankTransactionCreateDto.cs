using System.ComponentModel.DataAnnotations;

namespace BankofDotNet.DTOs.BankTransaction;

public class BankTransactionCreateDto
{
    [Required]
    public Guid AccountId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public string Amount { get; set; }
}
