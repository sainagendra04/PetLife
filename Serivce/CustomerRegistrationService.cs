using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetLife.Dto;
using PetLife.Dto.ErrorCodes;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Models.DBContext;
using System.Security.Cryptography;
using System.Text;

namespace PetLife.Serivce
{
    public class CustomerRegistrationService:IUserRegistration
    {
        private readonly PetLifeDBContext context;
        private readonly UserErrors errors;
        public CustomerRegistrationService(PetLifeDBContext _context, UserErrors _errors)
        {
            context = _context;
            errors = _errors;
        }

        public async Task<IActionResult> RegisterCustomer(RegisterCustomerDto dto)
        {
            //Check if user with username or email already exists
            if( await context.Users.AnyAsync(user=> user.UserName == dto.UserName || user.Email == dto.Email))
            {
                return new BadRequestObjectResult(errors.UserWithNameOrEmailExists);
            }
            
            string hashedPassword = HashPassword(dto.Password);

            // Create new User entity
            var user = new User
            { 
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = dto.Role.ToLower()
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Create new Customer entity linked to the User

            var customer = new Customer
            {
                UserId = user.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return new OkObjectResult(new { Message = "Customer registered successfully", CustomerID = customer.CustomerId});
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
    }
}
