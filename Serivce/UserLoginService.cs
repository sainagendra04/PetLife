using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetLife.Dto;
using PetLife.Dto.ErrorCodes;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Models.DBContext;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PetLife.Serivce
{
    public class UserLoginService : Controller, ICustomerLogin
    {
        private readonly PetLifeDBContext context;
        private readonly IConfiguration config;
        private readonly UserErrors errors;

        public UserLoginService(PetLifeDBContext _context, IConfiguration _config, UserErrors _errors)
        {
            context = _context;
            config = _config;
            errors = _errors;
        }

        public async Task<IActionResult> LoginCustomer(LoginDto dto)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.UserName == dto.UserName);
            if (user == null)
            {
                return new UnauthorizedObjectResult(new { Message = errors.InvalidUserCredentials });
            }
            if (user.PasswordHash != HashPassword(dto.Password))
            {
                return new UnauthorizedObjectResult(new { Message = errors.InvalidUserCredentials });
            }
            //Generate JWT token
            var token = GenerateJWTToken(user);
            return Ok(new { 
                Message = "Loggin Successfull...!", 
                Token = token,
                Role = user.Role
            });
        }

        private string HashPassword(string password)
        {
            //Implementing the security of password using SHA256 hashing algorithm
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder(hashedBytes.Length * 2);
            foreach (var b in hashedBytes)
                builder.AppendFormat("{0:x2}", b);
            return builder.ToString();
        }

        private string GenerateJWTToken(User user)
        {
            var claims = new[]
       {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
