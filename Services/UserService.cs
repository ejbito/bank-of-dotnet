using BankofDotNet.DTOs.User;
using BankofDotNet.Repository;
using BankofDotNet.Repository.Interface;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IdentityResult> DeleteAsync(Guid userId)
    {
        _logger.LogInformation($"Attempting to delete user: {userId}");

        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User not found: {userId}. Cannot delete.");
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        var result = await _userRepository.DeleteAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation($"User deleted: {userId}");
        }
        else
        {
            _logger.LogWarning($"Failed to delete user: {userId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return result;
    }

    public async Task<UserReadDto> GetProfileAsync(Guid userId)
    {
        _logger.LogInformation($"Retrieving profile for user: {userId}");

        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User not found: {userId}. Cannot retrieve profile.");
            throw new KeyNotFoundException($"User not found: {userId}.");
        }

        return new UserReadDto
        {
            UserId = userId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };
    }

    public async Task<IdentityResult> UpdatePasswordAsync(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto)
    {
        _logger.LogInformation($"Attempting to update password for user: {userId}");

        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User not found: {userId}. Cannot update password.");
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        var result = await _userRepository.ChangePasswordAsync(user, userUpdatePasswordDto.CurrentPassword, userUpdatePasswordDto.NewPassword);
        if (result.Succeeded)
        {
            _logger.LogInformation($"Password updated for user: {userId}");
        }
        else
        {
            _logger.LogWarning($"Failed to update password for user: {userId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return result;
    }
}
