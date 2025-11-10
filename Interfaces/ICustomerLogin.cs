using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;

namespace PetLife.Interfaces
{
    public interface ICustomerLogin
    {
        public Task<IActionResult> LoginCustomer(LoginDto dto);
    }
}
