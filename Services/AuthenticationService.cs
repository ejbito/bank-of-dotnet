using BankofDotNet.DTOs.Authentication;
using BankofDotNet.Repository;
using BankofDotNet.Repository.Interface;
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
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthenticationService(UserManager<User> userManager, IUserRepository userRepository, IConfiguration configuration)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<(IdentityResult Result, Guid UserId)> RegisterAsync(AuthenticationCreateDto dto)
    {
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
        };
        var result = await _userRepository.CreateAsync(user, dto.Password);
        return (result, user.Id);
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await _userRepository.FindByEmailAsync(username);
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
