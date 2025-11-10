using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;

namespace PetLife.Interfaces
{
    public interface IOrders
    {
        Task<IActionResult> PlaceOrder(OrderDetailDto dto);
    }
}
