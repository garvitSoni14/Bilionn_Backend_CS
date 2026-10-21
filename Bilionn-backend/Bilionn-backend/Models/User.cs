namespace Bilionn_backend.Models
{
    public class User: IAuditable
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; }
        public AccountStatus AccountStatus { get; set; } = AccountStatus.ACTIVE;
        public string? Name { get; set; }
        public Employee? Employee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
