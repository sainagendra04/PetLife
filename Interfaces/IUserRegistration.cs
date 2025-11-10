using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;

namespace PetLife.Interfaces
{
    public interface IUserRegistration
    {
        public Task<IActionResult> RegisterCustomer(RegisterCustomerDto dto);
    }
}
