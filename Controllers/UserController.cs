using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Serivce;

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
    }
}
