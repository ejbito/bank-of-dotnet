using BankofDotNet.DTOs.Account;
using BankofDotNet.Services.Interfaces;
using BankOfDotNet.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace BankofDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService, UserManager<User> userManager)
        {
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AccountCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            try
            {
                var account = await _accountService.CreateAsync(Guid.Parse(userId), dto);
                return Ok(account);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetAllUserAccounts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            try
            {
                var accounts = await _accountService.GetAllByUserIdAsync(Guid.Parse(userId));
                return Ok(accounts);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetUserAccount(string accountId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            try
            {
                var account = await _accountService.GetByIdAsync(Guid.Parse(accountId));
                if (account == null)
                {
                    return NotFound("Account not found.");
                }
                if (account.UserId != Guid.Parse(userId))
                {
                    return Unauthorized("You do not have permission to view this account.");
                }
                return Ok(account);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid account Id format.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update/{accountId}")]
        public async Task<IActionResult> Update(string accountId, [FromBody] AccountUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            try
            {
                var success = await _accountService.UpdateAsync(Guid.Parse(accountId), Guid.Parse(userId), dto);
                if (!success)
                {
                    return BadRequest("Update failed.");
                }
                return Ok("Account updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("updatePin/{accountId}")]
        public async Task<IActionResult> UpdatePin(string accountId, [FromBody] AccountUpdatePinDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            try
            {
                var success = await _accountService.UpdatePinAsync(Guid.Parse(accountId), Guid.Parse(userId), dto);
                if (!success)
                {
                    return BadRequest("PIN update failed.");
                }
                return Ok("PIN updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(string accountId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            try
            {
                var success = await _accountService.DeleteAsync(Guid.Parse(accountId), Guid.Parse(userId));
                if (!success)
                {
                    return BadRequest("Account deletion failed.");
                }
                return Ok("Account deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}