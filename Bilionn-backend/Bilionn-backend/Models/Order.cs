namespace Bilionn_backend.Models
{
    public class Order
    {
        public long Id { get; set; }
        public string? OrderId { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItem> Items { get; set; } = new();

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentDetails? PaymentDetails { get; set; }
    }
}
