using PaymentApi.Protos;

namespace OrderApi.GrpcService;

public class PaymentGrpcService
{
    private readonly PaymentProtoService.PaymentProtoServiceClient _paymentServiceClient;
    public PaymentGrpcService(PaymentProtoService.PaymentProtoServiceClient paymentServiceClient)
    {
        _paymentServiceClient = paymentServiceClient;
    }
    public async Task<string> CreatePayment(string orderId, long amount, string transactionId)
    {
        var request = new GetPaymentUrlRequest()
        {
            OrderId = orderId,
            Amount = amount,
            TransactionId = transactionId
        };
        var response = await _paymentServiceClient.CreatePaymentAsync(request);
        return response.Url;
    }
}