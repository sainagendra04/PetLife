namespace PetLife.Dto
{
    public class GetOrderItemDto
    {
        public Guid OrderItemId { get; set; }
        public Guid? PetId { get; set; }
        public string? PetName { get; set; }
        public Guid? FoodId { get; set; }
        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class GetOrderDetailDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<GetOrderItemDto> OrderItems { get; set; }
    }
}
