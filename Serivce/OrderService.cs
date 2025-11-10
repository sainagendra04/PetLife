using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetLife.Dto;
using PetLife.Interfaces;
using PetLife.Migrations;
using PetLife.Models;
using PetLife.Models.DBContext;

namespace PetLife.Serivce
{
    public class OrderService : Controller, IOrders
    {
        private readonly PetLifeDBContext context;
        public OrderService(PetLifeDBContext _context)
        {
            context = _context;
        }
        public async Task<IActionResult> PlaceOrder(OrderDetailDto dto)
        {
            if (dto.OrderItems == null || !dto.OrderItems.Any())
                return BadRequest("Order must contain at least one item.");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in dto.OrderItems)
            {
                var pet = await context.Pets.FindAsync(item.ItemId.Value);
                var food = await context.PetFoods.FindAsync(item.ItemId.Value);
                decimal itemPrice = 0;
                
                    if (pet != null)
                    {
                        itemPrice += pet.Price;
                    }
                    else if (food != null)
                        itemPrice += food.Price;
                    //return BadRequest($"Pet with ID {item.ItemId} is not available.");
                    //itemPrice += pet.Price;

                //if (item.FoodId.HasValue)
                //{
                //    var food = await context.PetFoods.FindAsync(item.FoodId.Value);
                //    if (food == null)
                //        return BadRequest($"Food with ID {item.FoodId} does not exist.");
                //    itemPrice += food.Price;
                //}
                var orderItem = new OrderItem
                {
                    PetId = pet?.PetId,
                    FoodId = food?.PetFoodId,
                    Quantity = item.Quantity,
                    Price = itemPrice,

                };
                totalAmount += itemPrice * item.Quantity;
                orderItems.Add(orderItem);
            }
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                ShippingAddress = dto.ShippingAddress,
                OrderItems = orderItems,

                TotalAmount = totalAmount
            };
            
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order created successfully",
                orderId = order.OrderId,
                totalAmount = order.TotalAmount
            });
        }
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found");
            var orderDetails = new GetOrderDetailDto
            {
                CustomerId = order.CustomerId,
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new GetOrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    PetId = oi.PetId,
                    PetName = oi.Pet?.Name,
                    FoodId = oi.FoodId,
                    FoodName = oi.Food?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList(),
                Status = order.Status
            };
            return Ok(orderDetails);
        }
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var orders = await context.Orders
                .Include(oi => oi.OrderItems)
                .Select( order => new GetOrderDetailDto
                {
                    CustomerId = order.CustomerId,
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    OrderItems = order.OrderItems.Select(oi => new GetOrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        PetId = oi.PetId,
                        PetName = oi.Pet.Name,
                        FoodId = oi.FoodId,
                        FoodName = oi.Food.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList(),
                    Status = order.Status
                }).ToListAsync();
            return Ok(orders);
        }
        public async Task<IActionResult> GetOrderDetailsByCustomer(Guid customerId)
        {
            var orders = await context.Orders
        .Where(o => o.CustomerId == customerId)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Pet)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Food)
        .ToListAsync();

            if (orders == null || !orders.Any())
                return NotFound("No orders found for this customer.");

            var response = orders.Select(order => new GetOrderDetailDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new GetOrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    PetId = oi.PetId,
                    PetName = oi.Pet?.Name,
                    FoodId = oi.FoodId,
                    FoodName = oi.Food?.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(response);
        }
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return NotFound("Order not found");

            context.Orders.Remove(order);
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
            await context.SaveChangesAsync();
            return Ok(new { message = $"Order with OrderId: {orderId} deleted successfully" });
        }
        public async Task<IActionResult> DeleteOrderItem(Guid orderItemId)
        {
            var orderItem = await context.OrderItems
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);

            if (orderItem == null)
                return NotFound("Order item not found.");
            var order = orderItem.Order;
            context.OrderItems.Remove(orderItem);
            await context.Entry(order).Collection(o => o.OrderItems).LoadAsync();
            order.TotalAmount = order.OrderItems
                .Where(oi => oi.OrderItemId != orderItemId)
                .Sum(oi => oi.Price * oi.Quantity);
            await context.SaveChangesAsync();
            return NoContent();
        }
        public async Task<IActionResult> UpdateOrder(Guid id, UpdateOrderDto dto)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound("Order not found");

            foreach (var itemDto in dto.OrderItems)
            {
                if (itemDto.OrderItemId.HasValue)
                {
                    var existingItem = order.OrderItems
                        .FirstOrDefault(oi => oi.OrderItemId == itemDto.OrderItemId.Value);

                    if (existingItem != null)
                    {
                        existingItem.PetId = itemDto.PetId;
                        existingItem.FoodId = itemDto.FoodId;
                        existingItem.Quantity = itemDto.Quantity;
                        existingItem.Price = itemDto.Price;
                    }
                }
                else
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        PetId = itemDto.PetId,
                        FoodId = itemDto.FoodId,
                        Quantity = itemDto.Quantity,
                        Price = itemDto.Price
                    });
                }
            }
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            await context.SaveChangesAsync();

            return Ok(order);
        }
    }
}
