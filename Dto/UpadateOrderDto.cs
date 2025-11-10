namespace PetLife.Dto
{
    public class UpdateOrderItemDto
    {
        public Guid? OrderItemId { get; set; }
        public Guid? PetId { get; set; }
        public Guid? FoodId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateOrderDto
    {
        public List<UpdateOrderItemDto> OrderItems { get; set; } = new();
    }

}
