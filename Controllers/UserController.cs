using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetLife.Dto;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Serivce;
using System.Security.Claims;

namespace PetLife.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserLoginService userLoginService;
        private readonly CustomerRegistrationService customerRegistrationService;
        private readonly UserService userService;
        public UserController(UserLoginService _userLoginService, CustomerRegistrationService _customerRegistrationService
            , UserService _userService)
        {
            userLoginService = _userLoginService;
            customerRegistrationService = _customerRegistrationService;
            userService = _userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegister([FromBody] RegisterCustomerDto dto)
        {
            return await customerRegistrationService.RegisterCustomer(dto);
        }
        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(LoginDto dto)
        {
            return await userLoginService.LoginCustomer(dto);
        }
        
        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetUserByUsername(string userName)
        {
            try
            {
                return await userService.GetCustomerByName(userName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                return await userLoginService.LogoutCustomer();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Logout failed", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("user/id/{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                return await userService.GetCustomerById(userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("user/email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                return await userService.GetCustomerByEmail(email);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllCustomers();
            return Ok(users);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // 1. Read the token from the cookie
            var oldToken = Request.Cookies["refreshToken"];
            if (oldToken == null)
            {
                return Unauthorized("Refresh token missing.");
            }

            // 2. Validate and rotate tokens (Logic usually in a Service)
            var result = await userLoginService.RotateTokens(oldToken);
            if (result == null) return Unauthorized("Invalid or expired refresh token.");

            // 3. Update the cookie with the NEW refresh token (Rotation)
            userLoginService.setRefreshTokenCookie(result.NewRefreshToken);

            // 4. Return new Access Token
            return Ok(new { token = result.NewAccessToken });
        }
    }
}
