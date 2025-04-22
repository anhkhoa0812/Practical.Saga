using PaymentApi.Dto;

namespace PaymentApi.Service;

public interface IMomoService
{
    string CreatePaymentUrl(MomoRequest momoRequest);
    
    Task<MomoIPNResponse> HandleIpnResponse(MomoResponse momoResponse);
}