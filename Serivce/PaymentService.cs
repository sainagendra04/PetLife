using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PetLife.Models;
using PetLife.Models.DBContext;
using Stripe;
using Stripe.Checkout;

namespace PetLife.Serivce
{
    public class PaymentService : Controller
    {
        private readonly PetLifeDBContext context;
        private readonly IConfiguration configuration;
        public PaymentService(PetLifeDBContext _context, IConfiguration _configuration)
        {
            context = _context;
            configuration = _configuration;
        }
        public string CreateStripeOrder(Guid orderId)
        {
            var order = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Pet)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null || order.OrderItems == null || !order.OrderItems.Any())
                throw new Exception("Order not found or has no items.");

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in order.OrderItems)
            {
                string itemName = item.Pet != null ? item.Pet.Name : item.Food?.Name ?? "Item";
                decimal itemPrice = item.Pet != null ? item.Pet.Price : item.Food?.Price ?? item.Price;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(itemPrice * 100), // cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = itemName,
                        },
                    },
                    Quantity = item.Quantity,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7290/success?orderId=" + orderId,
                CancelUrl = "https://localhost:7290/cancel?orderId=" + orderId,
                ClientReferenceId = orderId.ToString()
            };

            var service = new SessionService();
            return service.Create(options).Url;
        }
        public async Task<IActionResult> Success(Guid orderId, string transactionId = null)
        {
            var payment = await context.Payments
        .Include(p => p.Order)
        .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                // No existing payment found — create one
                var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                payment = new Payment
                {
                    OrderId = orderId,
                    Status = "Paid",
                    TransactionId = transactionId,
                    PaymentMethod = "Online",
                    Amount = order.TotalAmount,
                    UpdatedAt = DateTime.UtcNow
                };

                order.Status = "Processing";
                order.PaymentStatus = "Paid";

                context.Payments.Add(payment);
                await context.SaveChangesAsync();
            }
            else
            {
                // Payment found — just update it
                payment.Status = "Paid";
                payment.TransactionId = transactionId ?? payment.TransactionId;
                payment.PaymentMethod = "Online";
                payment.UpdatedAt = DateTime.UtcNow;

                if (payment.Order != null)
                {
                    payment.Order.Status = "Processing";
                    payment.Order.PaymentStatus = "Paid";
                    payment.Order.UpdatedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
            }
            return Ok("Payment successful! Your order is being processed.");
        }
        public async Task<IActionResult> Cancel(Guid orderId, string transactionId = null)
        {
            var payment = await context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                // No payment found — optionally create a record or return error
                var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                payment = new Payment
                {
                    OrderId = orderId,
                    Status = "Failed",
                    TransactionId = transactionId,
                    PaymentMethod = "Online",
                    UpdatedAt = DateTime.UtcNow
                };

                order.Status = "Cancelled";
                order.PaymentStatus = "Failed";
                order.UpdatedAt = DateTime.UtcNow;

                context.Payments.Add(payment);
            }
            else
            {
                // Payment exists — update it
                payment.Status = "Failed";
                payment.TransactionId = payment.TransactionId ?? null;
                payment.PaymentMethod = "Online";
                payment.UpdatedAt = DateTime.UtcNow;

                if (payment.Order != null)
                {
                    payment.Order.Status = "Cancelled";
                    payment.Order.PaymentStatus = "Failed";
                    payment.Order.UpdatedAt = DateTime.UtcNow;
                }
            }

            return Ok("Payment was cancelled.");
        }
        public async Task<IActionResult> HandleStripeEvent(Event stripeEvent)
        {
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var orderId = Guid.Parse(session.ClientReferenceId);

                    if (session.PaymentStatus == "paid")
                    {
                        await Success(orderId, session.PaymentIntentId);
                    }
                    else
                    {
                        await Cancel(orderId, "Payment not completed");
                    }
                }
                return Ok();
        }
    }
}
