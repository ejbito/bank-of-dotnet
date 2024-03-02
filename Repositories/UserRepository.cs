using BankofDotNet.Repository.Interface;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));
            }

            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null for deletion.");
            }

            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));
            }

            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<User> FindByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            return user;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null for password change.");
            }
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                throw new ArgumentException("Current password cannot be null or whitespace.", nameof(currentPassword));
            }
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("New password cannot be null or whitespace.", nameof(newPassword));
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result;
        }
    }
}
