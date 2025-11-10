namespace PetLife.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // e.g. Processing, Shipped, Delivered, Cancelled
        public string PaymentStatus { get; set; } = "Unpaid"; // e.g. Unpaid, Paid, Refunded
        public string ShippingAddress { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }

    }
}
