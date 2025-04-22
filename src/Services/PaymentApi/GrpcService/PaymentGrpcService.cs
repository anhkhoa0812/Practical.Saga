using Grpc.Core;
using PaymentApi.Dto;
using PaymentApi.Entities;
using PaymentApi.Protos;
using PaymentApi.Repositories;
using PaymentApi.Service;

namespace PaymentApi.GrpcService;

public class PaymentGrpcService : PaymentProtoService.PaymentProtoServiceBase
{
    private readonly IMomoService _momoService;
    private readonly IPaymentRepository _paymentRepository;
    public PaymentGrpcService(IMomoService momoService, IPaymentRepository paymentRepository)
    {
        _momoService = momoService;
        _paymentRepository = paymentRepository;
    }
    public override async Task<GetPaymentUrlResponse> CreatePayment(GetPaymentUrlRequest request, ServerCallContext context)
    {
        var response = new GetPaymentUrlResponse();
        var payment = new Payment()
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.Parse(request.OrderId),
            PaymentDateTime = DateTime.UtcNow,
            Amount = request.Amount,
            TransactionId = request.TransactionId
        };
        var momoRequest = new MomoRequest()
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            OrderInfo = "kkk",
            ExtraData = "",
            RequestId = request.TransactionId
        };
        var paymentUrl = _momoService.CreatePaymentUrl(momoRequest);
        if (!string.IsNullOrEmpty(paymentUrl))
        {
            var paymentEntity = await _paymentRepository.CreatePayment(payment);
            if (paymentEntity != null)
            {
                response.Url = paymentUrl;
                return response;
            }
        }

        return new GetPaymentUrlResponse();
    }
}