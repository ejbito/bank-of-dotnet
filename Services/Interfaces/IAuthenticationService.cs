using BankofDotNet.DTOs.Authentication;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Services.Interfaces;

public interface IAuthenticationService
{
    Task<(IdentityResult Result, Guid UserId)> RegisterAsync(AuthenticationCreateDto dto);
    Task<string> SignInAsync(string username, string password);
}
