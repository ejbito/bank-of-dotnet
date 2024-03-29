﻿using BankofDotNet.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Services.Interfaces;

public interface IUserService
{
    Task<UserReadDto> GetProfileAsync(Guid userId);
    Task<IdentityResult> UpdatePasswordAsync(Guid userId, UserUpdatePasswordDto dto);
    Task<IdentityResult> DeleteAsync(Guid userId);
}
