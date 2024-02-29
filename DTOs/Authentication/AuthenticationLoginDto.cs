using System.ComponentModel.DataAnnotations;

namespace BankofDotNet.DTOs.Authentication;

public class AuthenticationLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
