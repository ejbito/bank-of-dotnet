using BankofDotNet.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Services.Interfaces;

public interface IUserService
{
    Task<(IdentityResult result, Guid userId)> RegisterUserAsync(UserCreateDto userCreateDto);
    Task<UserReadDto> GetUserProfileAsync(Guid userId);
    Task<IdentityResult> UpdateUserPasswordAsync(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto);
    Task<IdentityResult> DeleteUserAsync(Guid userId);
}
