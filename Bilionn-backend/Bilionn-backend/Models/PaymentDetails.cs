namespace Bilionn_backend.Models;

public class PaymentDetails
{
    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpaySignature { get; set; }
    public PaymentStatus? Status { get; set; }
}