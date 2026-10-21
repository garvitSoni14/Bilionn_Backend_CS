namespace Bilionn_backend.Models
{
    public class Category: IAuditable
    {
        public long Id { get; set; }
        public string? CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BgColor { get; set; }
        public byte[]? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
