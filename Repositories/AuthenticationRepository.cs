using BankofDotNet.Repositories.Interfaces;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;

namespace BankofDotNet.Repositories;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly UserManager<User> _userManager;

    public AuthenticationRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}
