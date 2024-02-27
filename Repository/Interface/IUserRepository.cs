using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Repository.Interface;

public interface IUserRepository
{
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<IdentityResult> DeleteAsync(User user);
    Task<User> FindByEmailAsync(string email);
    Task<User> FindByIdAsync(Guid userId);
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
}
