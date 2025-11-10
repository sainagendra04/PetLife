namespace PetLife.Models
{
    public class Pet
    {
        public Guid PetId { get; set; }
        public string Name { get; set; } = null!;
        public Guid PetCategoryId { get; set; }
        public PetCategory Category { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; } = true;
    }
}
