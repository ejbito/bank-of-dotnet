using System.ComponentModel.DataAnnotations;

namespace BankofDotNet.DTOs.User;

public class UserCreateDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
