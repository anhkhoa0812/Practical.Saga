using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PaymentApi.Dto;
using PaymentApi.Util;
using Shared.Configurations;

namespace PaymentApi.Service;

public class MomoService : IMomoService
{
    private readonly MomoOptions _options;
    
    public MomoService(IOptions<MomoOptions> options)
    {
        _options = options.Value;
    }
    public string CreatePaymentUrl(MomoRequest momoRequest)
    {
        string endPoint = _options.MomoApi;
        
        string requestType = "captureWallet";
        
        var rawHash = MomoUtil.MakeRawHashRequest(_options, momoRequest, requestType);
        
        var signature = MomoUtil.SignSHA256(rawHash, _options.SecretKey);
        
        JObject message = new JObject
        {
            { "partnerCode", _options.PartnerCode },
            { "partnerName", "Test" },
            { "storeId", "MomoTestStore" },
            { "requestId", momoRequest.RequestId },
            { "amount", momoRequest.Amount },
            { "orderId", momoRequest.OrderId },
            { "orderInfo", momoRequest.OrderInfo },
            { "redirectUrl", _options.RedirectUrl },
            { "ipnUrl", _options.IpnUrl },
            { "lang", "en" },
            { "extraData", momoRequest.ExtraData },
            { "requestType", requestType },
            { "signature", signature }
        };
        
        string responseFromMomo = MomoUtil.SendPaymentRequest(endPoint, message.ToString());
        JObject jmessage = JObject.Parse(responseFromMomo);
        if (jmessage.GetValue("payUrl") != null)
        {
            return jmessage.GetValue("payUrl").ToString();
        }
        else
        {
            return jmessage.GetValue("message").ToString();
        }
    }

    public async Task<MomoIPNResponse> HandleIpnResponse(MomoResponse momoResponse)
    {
        MomoIPNResponse momoIPNResponseResponse = new MomoIPNResponse();
        var rawHash = MomoUtil.MakeRawHashResponse(_options, momoResponse);
        
        var signature = MomoUtil.SignSHA256(rawHash, _options.SecretKey);
        if (signature.Equals(momoResponse.Signature))
        {
            momoIPNResponseResponse.PartnerCode = momoResponse.PartnerCode;
            momoIPNResponseResponse.RequestId = momoResponse.RequestId.ToString();
            momoIPNResponseResponse.OrderId = momoResponse.OrderId.ToString();
            momoIPNResponseResponse.ResultCode = momoResponse.ResultCode;
            momoIPNResponseResponse.Message = momoResponse.Message;
            momoIPNResponseResponse.ResponseTime = momoResponse.ResponseTime;
            momoIPNResponseResponse.ExtraData = momoResponse.ExtraData;
            rawHash = "accessKey=" + _options.AccessKey +
                      "&extraData=" + momoResponse.ExtraData +
                      "&message=" + momoResponse.Message +
                      "&orderId=" + momoResponse.OrderId +
                      "&partnerCode=" + momoResponse.PartnerCode +
                      "&requestId=" + momoResponse.RequestId +
                      "&responseTime=" + momoResponse.ResponseTime +
                      "&resultCode=" + momoResponse.ResultCode;
            momoIPNResponseResponse.Signature = MomoUtil.SignSHA256(rawHash, _options.SecretKey);
            return momoIPNResponseResponse;
        }
        else
        {
            return null;
        }
    }
}