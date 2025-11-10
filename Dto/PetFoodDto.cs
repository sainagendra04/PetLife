using PetLife.Models;

namespace PetLife.Dto
{
    public class PetFoodDto
    {
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
