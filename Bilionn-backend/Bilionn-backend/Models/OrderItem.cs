namespace Bilionn_backend.Models
{
    public class OrderItem
    {
        public long Id { get; set; }
        public string? ItemId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
