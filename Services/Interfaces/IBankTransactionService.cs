using BankofDotNet.DTOs.BankTransaction;

namespace BankofDotNet.Services.Interfaces;

public interface IBankTransactionService
{
    Task<BankTransactionReadDto> DepositAsync(Guid accountId, decimal amount);
    Task<BankTransactionReadDto> TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount);
    Task<BankTransactionReadDto> WithdrawAsync(Guid accountId, decimal amount);
    Task<IEnumerable<BankTransactionReadDto>> GetBankTransactionByAccountId(Guid accountId);
    Task<BankTransactionReadDto> GetBankTransactionByTransactionId(Guid transactionId);
}
