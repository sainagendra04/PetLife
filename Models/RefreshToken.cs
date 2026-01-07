namespace PetLife.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; } // Used for rotation detection
    }

    public record TokenResponse(string NewAccessToken, string NewRefreshToken);

}
