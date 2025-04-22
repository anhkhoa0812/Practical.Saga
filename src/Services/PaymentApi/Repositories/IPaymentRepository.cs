using PaymentApi.Entities;

namespace PaymentApi.Repositories;

public interface IPaymentRepository
{
    Task<Payment> CreatePayment(Payment payment);
}