using PetLife.Models;

namespace PetLife.Dto
{
    public class PetDto
    {
        public string Name { get; set; } = null!;
        public Guid PetCategoryId { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; } = true;
    }
}
