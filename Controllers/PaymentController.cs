using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetLife.Serivce;
using Stripe;

namespace PetLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService paymentService;
        public PaymentController(PaymentService _paymentService)
        {
            paymentService = _paymentService;
        }
        [HttpPost("create-order/{orderId}")]
        public IActionResult CreateOrder(Guid orderId)
        {
            var session = paymentService.CreateStripeOrder(orderId);
            return Ok(new { url = session });
        }
        //[HttpGet("success")]
        //public IActionResult Success(Guid orderId)
        //{
        //    return paymentService.Success(orderId).Result;
        //}
        //[HttpGet("cancel")]
        //public IActionResult Cancel(Guid orderId)
        //{
        //    return paymentService.Cancel(orderId).Result;
        //}
        [HttpPost("Webhooks")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeSignature = Request.Headers["Stripe-Signature"];
                const string webhookSecret = "{Use webhook from your stipe dashboard}";// 🔑 from Stripe Dashboard

                if (string.IsNullOrEmpty(stripeSignature))
                    return BadRequest("Missing Stripe-Signature header");

                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret, throwOnApiVersionMismatch: false);

                await paymentService.HandleStripeEvent(stripeEvent);

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
        }
    }
}
