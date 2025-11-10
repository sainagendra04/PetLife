namespace PetLife.Dto
{
    public class OrderItemDto
    {
        public Guid? ItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class OrderDetailDto
    {
        public Guid CustomerId { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
