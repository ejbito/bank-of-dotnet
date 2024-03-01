using BankofDotNet.DTOs.Authentication;
using BankofDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankofDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(AuthenticationCreateDto dto)
        {
            var (result, userId) = await _authenticationService.RegisterAsync(dto);
            if (result.Succeeded)
            {
                return Ok(new { UserId = userId });
            }
            return BadRequest(result.Errors);
        }


        [HttpPost("signIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(AuthenticationLoginDto dto)
        {
            var token = await _authenticationService.SignInAsync(dto.Email, dto.Password);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid login attempt.");
            }
            return Ok(new { Token = token });
        }
    }
}
