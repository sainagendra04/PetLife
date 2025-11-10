namespace PetLife.Models
{
    public class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public Guid? PetId { get; set; }
        public Pet Pet { get; set; }
        public Guid? FoodId { get; set; }
        public PetFood? Food { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
