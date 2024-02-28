using System.ComponentModel.DataAnnotations;

namespace BankofDotNet.DTOs.User;

public class UserUpdatePasswordDto
{
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
}
