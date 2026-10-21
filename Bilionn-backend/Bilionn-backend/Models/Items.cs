namespace Bilionn_backend.Models
{
    public class Items: IAuditable
    {
        public long Id { get; set; }
        public string? ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
        public Category Category { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
