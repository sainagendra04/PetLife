using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using PetLife.Dto;
using PetLife.Dto.ErrorCodes;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Models.DBContext;

namespace PetLife.Serivce
{
    public class UserService : Controller, IUser
    {
        private readonly PetLifeDBContext context;
        private readonly UserErrors errors;
        public UserService(PetLifeDBContext _context, UserErrors _errors)
        {
            context = _context;
            errors = _errors;
        }

        public async Task<IEnumerable<UserDetailsDto>> GetAllCustomers()
        {
            var users = await context.Customers.Include(c => c.User)
                .Select(user => new UserDetailsDto
                {
                    UserId = user.UserId,
                    CustomerId = user.CustomerId,
                    UserName = user.User.UserName,
                    Email = user.User.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address
                })
                .ToListAsync();
            return users;
        }

        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            var user = await context.Customers.Include(c => c.User)
                .Where(c => c.User.Email == email)
                .Select(user => new UserDetailsDto 
                {
                    UserId = user.UserId,
                    CustomerId = user.CustomerId,
                    UserName = user.User.UserName,
                    Email = user.User.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address
                })
                .FirstOrDefaultAsync();
            if (user == null) {
                throw new Exception(errors.EmailNotFound);
            }
            return Ok(user);
        }

        public async Task<IActionResult> GetCustomerById(Guid userId)
        {
            var user = await context.Customers.Include(c => c.User)
                .Where(c => c.UserId == userId)
                .Select(user => new UserDetailsDto
                {
                    UserId = user.UserId,
                    CustomerId = user.CustomerId,
                    UserName = user.User.UserName,
                    Email = user.User.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address
                })
                .FirstOrDefaultAsync();
            if (user == null) {
                throw new Exception(errors.UserNotFoundError);
            }
            return Ok(user);
        }

        public async Task<IActionResult> GetCustomerByName(string username)
        {
            var user = await context.Customers.Include(c => c.User)
                            .Where(c => c.User.UserName == username)
                            .Select(user => new UserDetailsDto
                            {
                                UserId = user.UserId,
                                CustomerId = user.CustomerId,
                                UserName = user.User.UserName,
                                Email = user.User.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                PhoneNumber = user.PhoneNumber,
                                Address = user.Address
                            })
                            .FirstOrDefaultAsync(); if (user == null)
            {
                throw new Exception(errors.UsernameNotFound);
            }
            return Ok(user);
        }
    }
}
