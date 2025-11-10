namespace PetLife.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Customer Customer { get; set; }
        public string Role { get; set; } = null!;
    }
}
