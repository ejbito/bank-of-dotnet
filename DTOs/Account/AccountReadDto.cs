using BankOfDotNet.Enums;

namespace BankofDotNet.DTOs.Account;

public class AccountReadDto
{
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; }
}
