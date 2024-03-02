using BankofDotNet.DTOs.BankTransaction;
using BankofDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace BankofDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BankTransactionController : ControllerBase
    {
        private readonly IBankTransactionService _bankTransactionService;

        public BankTransactionController(IBankTransactionService bankTransactionService)
        {
            _bankTransactionService = bankTransactionService;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] BankTransactionCreateDto dto)
        {
            try
            {
                var accountId = dto.AccountId;
                var amount = dto.Amount;
                var transaction = await _bankTransactionService.DepositAsync(accountId, amount);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] BankTransactionCreateDto dto)
        {
            try
            {
                var accountId = dto.AccountId;
                var amount = dto.Amount;
                var transaction = await _bankTransactionService.WithdrawAsync(accountId, amount);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] BankTransactionTransferDto dto)
        {
            try
            {
                var fromAccountId = dto.FromAccountId;
                var toAccountId = dto.ToAccountId;
                var amount = dto.Amount;
                var transaction = await _bankTransactionService.TransferAsync(fromAccountId, toAccountId, amount);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetAllBankTransactions(Guid accountId)
        {
            try
            {
                var transactions = await _bankTransactionService.GetTransactionsByAccountAsync(accountId);
                return Ok(transactions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("transaction/{transactionId}")]
        public async Task<IActionResult> GetBankTransaction(Guid transactionId)
        {
            try
            {
                var transactions = await _bankTransactionService.GetTransactionByIdAsync(transactionId);
                return Ok(transactions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
