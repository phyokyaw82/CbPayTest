using CbPayTest.Models;

namespace CbPayTest.Services;

public class CbPayService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public CbPayService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<CbPayResponse> RequestPaymentAsync(object payload)
    {
        var url = _config["CbPay:BaseUrl"] + "request-payment-order.service";

        var res = await _http.PostAsJsonAsync(url, payload);
        var data = await res.Content.ReadFromJsonAsync<CbPayResponse>();

        return data;
    }

    public async Task<string> CheckStatusAsync(object payload)
    {
        var url = _config["CbPay:BaseUrl"] + "checkstatus-webpayment.service";

        var res = await _http.PostAsJsonAsync(url, payload);
        return await res.Content.ReadAsStringAsync();
    }
}