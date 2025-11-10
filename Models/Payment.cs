namespace PetLife.Models
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = null!; // e.g. Card, UPI, PayPal
        public string Status { get; set; } = "Pending"; // e.g. Pending, Completed, Failed, Refunded
        public string TransactionId { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsRefund { get; set; } = false;
        public DateTime UpdatedAt { get; set; }
    }
}
