using System.ComponentModel.DataAnnotations;

namespace BankofDotNet.DTOs.Account;

public class AccountUpdatePinDto
{
    [Required]
    [DataType(DataType.Password)]
    [RegularExpression("^[0-9]{4}$", ErrorMessage = "Transaction PIN must be exactly 4 digits.")]
    public string CurrentPin { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression("^[0-9]{4}$", ErrorMessage = "Transaction PIN must be exactly 4 digits.")]
    public string NewPin { get; set; }
}
