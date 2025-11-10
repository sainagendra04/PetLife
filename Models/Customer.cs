namespace PetLife.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
