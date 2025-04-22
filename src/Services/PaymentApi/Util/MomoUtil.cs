using System.Net;
using System.Security.Cryptography;
using System.Text;
using PaymentApi.Dto;
using Shared.Configurations;

namespace PaymentApi.Util;

public static class MomoUtil
{
    // private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
    
    public static string SignSHA256(string message, string key)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(key);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            string hex = BitConverter.ToString(hashmessage);
            hex = hex.Replace("-", "").ToLower();
            return hex;
        }
    }

    public static string MakeRawHashRequest(MomoOptions options, MomoRequest request, string requestType)
    {
        string rawHash = "accessKey=" + options.AccessKey +
                         "&amount=" + request.Amount +
                         "&extraData=" + request.ExtraData +
                         "&ipnUrl=" + options.IpnUrl +
                         "&orderId=" + request.OrderId +
                         "&orderInfo=" + request.OrderInfo +
                         "&partnerCode=" + options.PartnerCode +
                         //"&redirectUrl=" + redirectUrl +
                         "&redirectUrl=" + options.RedirectUrl +
                         "&requestId=" + request.RequestId +
                         "&requestType=" + requestType;
        return rawHash;
    }
    
    public static string SendPaymentRequest(string endpoint, string postJsonString)
    {

        try
        {
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);

            var postData = postJsonString;

            var data = Encoding.UTF8.GetBytes(postData);

            httpWReq.ProtocolVersion = HttpVersion.Version11;
            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/json";

            httpWReq.ContentLength = data.Length;
            httpWReq.ReadWriteTimeout = 30000;
            httpWReq.Timeout = 22000;
            Stream stream = httpWReq.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

            string jsonresponse = "";

            using (var reader = new StreamReader(response.GetResponseStream()))
            {

                string temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    jsonresponse += temp;
                }
            }
            return jsonresponse;
        }
        catch (WebException e)
        {
            return e.Message;
        }
    }

    public static string MakeRawHashResponse(MomoOptions options, MomoResponse response)
    {
        var rawHash = "accessKey=" + options.AccessKey +
                      "&amount=" + response.Amount +
                      "&extraData=" + response.ExtraData +
                      "&message=" + response.Message +
                      "&orderId=" + response.OrderId +
                      "&orderInfo=" + response.OrderInfo +
                      "&orderType=" + response.OrderType +
                      "&partnerCode=" + response.PartnerCode +
                      "&payType=" + response.PayType +
                      "&requestId=" + response.RequestId +
                      "&responseTime=" + response.ResponseTime +
                      "&resultCode=" + response.ResultCode +
                      "&transId=" + response.TransId;
        return rawHash;
    }
}