using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Repository.Interface;

public interface IUserRepository
{
    public Task<IdentityResult> CreateAsync(User user, string password);
    public Task<IdentityResult> DeleteAsync(User user);
    public Task<User> FindByEmailAsync(string email);
    public Task<User> FindByIdAsync(Guid userId);
    public Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
}
