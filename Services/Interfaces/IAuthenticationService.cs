namespace BankofDotNet.Services.Interfaces;

public interface IAuthenticationService
{
    Task<string> SignInAsync(string username, string password);
    // Task<string> SignOutAsync();
}
