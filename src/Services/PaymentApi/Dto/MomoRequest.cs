using System.Text.Json.Serialization;

namespace PaymentApi.Dto;

public class MomoRequest
{
    [JsonPropertyName("orderInfo")]
    public string OrderInfo { get; set; }
    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } //payment ID in db
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; }
    [JsonPropertyName("extraData")]
    public string ExtraData { get; set; }
}