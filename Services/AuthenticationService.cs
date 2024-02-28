using BankofDotNet.Repositories;
using BankofDotNet.Repositories.Interfaces;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankofDotNet.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly AuthenticationRepository _authenticationRepository;
    private readonly IConfiguration _configuration;

    public AuthenticationService(UserManager<User> userManager, AuthenticationRepository authenticationRepository, IConfiguration configuration)
    {
        _userManager = userManager;
        _authenticationRepository = authenticationRepository;
        _configuration = configuration;
    }

    public async Task<string> SignInAsync(string username, string password)
    {
        var user = await _authenticationRepository.FindByEmailAsync(username);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            return GenerateToken(user);
        }
        throw new KeyNotFoundException();
    }

    private string GenerateToken(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };
        var token = new JwtSecurityToken(
            issuer: _configuration["Authentication:Issuer"],
            audience: _configuration["Authentication:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
