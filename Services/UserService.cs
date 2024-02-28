using BankofDotNet.DTOs.User;
using BankofDotNet.Repository;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly UserRepository _userRepository;

    public UserService(UserManager<User> userManager, UserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<IdentityResult> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }
        return await _userRepository.DeleteAsync(user);
    }

    public async Task<UserReadDto> GetUserProfileAsync(Guid userId)
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

    public async Task<(IdentityResult result, Guid userId)> RegisterUserAsync(UserCreateDto userCreateDto)
    {
        var user = new User
        {
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            Email = userCreateDto.Email,
        };
        var result = await _userRepository.CreateAsync(user, userCreateDto.Password);
        return (result, user.Id);
    }

    public async Task<IdentityResult> UpdateUserPasswordAsync(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }
        return await _userRepository.ChangePasswordAsync(user, userUpdatePasswordDto.CurrentPassword, userUpdatePasswordDto.NewPassword);
    }
}
