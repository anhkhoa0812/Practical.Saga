using Microsoft.AspNetCore.Mvc;
using PaymentApi.Dto;
using PaymentApi.Service;

namespace PaymentApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IMomoService _momoService;

    public PaymentController(IMomoService momoService)
    {
        _momoService = momoService;
    }

    [HttpPost("create")]
    public string CreatePayment([FromBody] MomoRequest request)
    {
        var result = _momoService.CreatePaymentUrl(request);
        return result;
    }
}