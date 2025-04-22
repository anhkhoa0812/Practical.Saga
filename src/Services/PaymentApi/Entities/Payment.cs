namespace PaymentApi.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public string TransactionId { get; set; } = default!; // Transaction ID third party return
    public DateTime PaymentDateTime { get; set; }
    public long Amount { get; set; }
    public Guid OrderId { get; set; }
}