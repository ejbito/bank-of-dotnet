using BankOfDotNet.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankofDotNet.DTOs.Account;

public class AccountUpdateDto
{
    [Required]
    public AccountType AccountType { get; set; }
}
