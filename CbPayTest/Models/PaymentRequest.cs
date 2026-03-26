namespace CbPayTest.Models;

public class PaymentRequest
{
    public string OrderId { get; set; } = "AA1111";
    public string OrderDetails { get; set; } = "CB Test Order 001";
    public decimal Amount { get; set; } = 800.00M;
}

