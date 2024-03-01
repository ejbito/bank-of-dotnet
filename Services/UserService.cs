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

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IdentityResult> DeleteAsync(Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }
        return await _userRepository.DeleteAsync(user);
    }

    public async Task<UserReadDto> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException();
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
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }
        return await _userRepository.ChangePasswordAsync(user, userUpdatePasswordDto.CurrentPassword, userUpdatePasswordDto.NewPassword);
    }
}
