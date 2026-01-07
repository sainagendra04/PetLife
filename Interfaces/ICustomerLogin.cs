using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;
using PetLife.Models;

namespace PetLife.Interfaces
{
    public interface ICustomerLogin
    {
        public Task<IActionResult> LoginCustomer(LoginDto dto);
        Task<TokenResponse?> RotateTokens(string? oldToken);
        Task<string> CreateAndSaveRefreshToken(Guid userId);
    }
}
