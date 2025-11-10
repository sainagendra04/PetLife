namespace PetLife.Models
{
    public class PetFood
    {
        public Guid PetFoodId { get; set; }
        public string Name { get; set; } = null!;
        public Guid PetCategoryId { get; set; }
        public PetCategory Category { get; set; } = null!;
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
