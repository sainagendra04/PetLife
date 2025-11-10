using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;
using PetLife.Serivce;

namespace PetLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService orderService;
        public OrderController(OrderService _orderService)
        {
            orderService = _orderService;
        }
        [HttpPost("placeorder")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDetailDto orderItem)
        {
            return await orderService.PlaceOrder(orderItem);
        }
        [HttpGet("ordersbyorderId/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
        {
            return await orderService.GetOrderDetails(orderId);
        }
        [HttpGet("getallorders")]
        public async Task<IActionResult> GetAllOrderDetails()
        { 
            return await orderService.GetAllOrderDetails();
        }
        [HttpGet("ordersbycustomerId/{customerId}")]
        public async Task<IActionResult> GetOrderDetailsByCustomer(Guid customerId)
        {
            return await orderService.GetOrderDetailsByCustomer(customerId);
        }
        [HttpDelete("cancelorder/{orderId}")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            return await orderService.DeleteOrder(orderId);
        }
        [HttpDelete("deleteorderitem/{orderItemId}")]
        public async Task<IActionResult> DeleteOrderItem(Guid orderItemId)
        {
            return await orderService.DeleteOrderItem(orderItemId);
        }
        [HttpPut("updateorderitem/{orderItemId}")]
        public async Task<IActionResult> UpdateOrderItem(Guid orderItemId, [FromBody] UpdateOrderDto updatedOrderItem)
        {
            return await orderService.UpdateOrder(orderItemId, updatedOrderItem);
        }
    }
}
