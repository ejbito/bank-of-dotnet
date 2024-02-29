using BankOfDotNet.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankofDotNet.DTOs.Account;

public class AccountCreateDto
{
    [Required]
    public AccountType AccountType { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression("^[0-9]{4}$", ErrorMessage = "Transaction PIN must be exactly 4 digits.")]
    public string Pin { get; set; }
}
