using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BankOfDotNet.Models;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
