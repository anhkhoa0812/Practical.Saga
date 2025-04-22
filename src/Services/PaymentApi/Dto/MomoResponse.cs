using System.Text.Json.Serialization;

namespace PaymentApi.Dto;

public class MomoResponse
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; }
    [JsonPropertyName("orderId")]
    public Guid OrderId { get; set; }
    [JsonPropertyName("requestId")]
    public Guid RequestId { get; set; }
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    [JsonPropertyName("orderInfo")]
    public string OrderInfo { get; set; }
    [JsonPropertyName("orderType")]
    public string OrderType { get; set; }
    [JsonPropertyName("transId")]
    public long TransId { get; set; }
    [JsonPropertyName("resultCode")]
    public int ResultCode { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("payType")]
    public string PayType { get; set; }
    [JsonPropertyName("responseTime")]
    public long ResponseTime { get; set; }
    [JsonPropertyName("extraData")]
    public string ExtraData { get; set; }
    [JsonPropertyName("signature")]
    public string Signature { get; set; }
}