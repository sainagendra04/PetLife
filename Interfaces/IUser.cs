using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;
using PetLife.Models;

namespace PetLife.Interfaces
{
    public interface IUser
    {
        public Task<IActionResult> GetCustomerByName(string username);
        public Task<IActionResult> GetCustomerByEmail(string email);
        public Task<IActionResult> GetCustomerById(Guid userId);
        public Task<IEnumerable<UserDetailsDto>> GetAllCustomers();

    }
}
