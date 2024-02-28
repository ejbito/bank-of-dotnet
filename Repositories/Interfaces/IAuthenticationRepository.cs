using BankOfDotNet.Models;

namespace BankofDotNet.Repositories.Interfaces;

public interface IAuthenticationRepository
{
    Task<User> FindByEmailAsync(string email);
}
