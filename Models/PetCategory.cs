namespace PetLife.Models
{
    public class PetCategory
    {
        public Guid PetCategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public ICollection<Pet> Pets { get; set; }
        public ICollection<PetFood> PetFood { get; set; }
    }
}
