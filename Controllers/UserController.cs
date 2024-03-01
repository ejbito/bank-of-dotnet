using BankofDotNet.DTOs.User;
using BankofDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankofDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            var profile = await _userService.GetProfileAsync(Guid.Parse(userId));
            if (profile == null)
            {
                return NotFound("User not found.");
            }
            return Ok(profile);
        }

        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword(UserUpdatePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty (userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            var result = await _userService.UpdatePasswordAsync(Guid.Parse(userId), dto);
            if (result.Succeeded)
            {
                return Ok("Password updated successfully.");
            }
            return BadRequest(result.Errors);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid User Id.");
            }
            var result = await _userService.DeleteAsync(Guid.Parse(userId));
            if (result.Succeeded)
            {
                return Ok("User deleted successfully.");
            }
            return BadRequest(result.Errors);
        }
    }
}
