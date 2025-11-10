namespace PetLife.Dto
{
    public class PetFoodListDto
    {
        public Guid PetFoodId { get; set; }
        public string Name { get; set; } = null!;
        public Category Category { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
