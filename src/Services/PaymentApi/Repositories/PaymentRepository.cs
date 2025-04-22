using PaymentApi.Entities;

namespace PaymentApi.Repositories;

public class PaymentRepository  : IPaymentRepository
{
    private readonly PaymentDbContext _dbContext;

    public PaymentRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Payment> CreatePayment(Payment payment)
    {
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();
        return payment;
    }
}