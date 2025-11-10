using PetLife.Models;

namespace PetLife.Dto
{
    public class PetDetailsDto
    {
        public string Name { get; set; }
        public PetCategoryDto Category { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; } = true;
    }
    public class PetCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
    public class PetAndCategoryDto
    {
        public string PetName { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
