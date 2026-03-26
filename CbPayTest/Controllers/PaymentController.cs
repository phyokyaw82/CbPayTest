using CbPayTest.Models;
using CbPayTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace CbPayTest.Controllers;

public class PaymentController : Controller
{
    private readonly CbPayService _service;
    private readonly IConfiguration _config;

    public PaymentController(CbPayService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    // Step 1: Show form
    public IActionResult Index()
    {
        return View();
    }

    // Step 2: Create Payment Order
    [HttpPost]
    public async Task<IActionResult> CreatePayment(PaymentRequest req)
    {
        var payload = new
        {
            authenToken = _config["CbPay:AuthToken"],
            ecommerceId = _config["CbPay:EcommerceId"],
            transactionType = _config["CbPay:TransactionType"],
            orderId = req.OrderId,
            orderDetails = req.OrderDetails,
            amount = req.Amount.ToString("0.00"),
            currency = _config["CbPay:Currency"],
            notifyUrl = $"{Request.Scheme}://{Request.Host}/Payment/Callback",
            signature = _config["CbPay:Signature"],
            subMerId = _config["CbPay:SubMerId"]
        };

        var generateRefOrder = await _service.RequestPaymentAsync(payload);

        // STEP 3: Redirect to CBPay Deeplink (IMPORTANT PART)
        var deeplink = $"cbuat://pay?keyreference={generateRefOrder}";

        return Redirect(deeplink);
    }

    // Step 4: CBPay backend calls this after success
    [HttpPost]
    public async Task<IActionResult> Callback([FromBody] object data)
    {
        try
        {
            Console.WriteLine($"CBPay Callback: {data}");

            // 1. parse data
            // 2. verify signature (VERY important)
            // 3. update order in DB
            // 4. avoid duplicate processing

            return Ok(); // always return OK if received
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            // still return OK or CBPay may retry forever
            return Ok();
        }
    }

    // Step 5: CBPay redirects user back here
    [HttpGet]
    public IActionResult CbPaySuccess(string keyreference)
    {
        ViewBag.KeyReference = keyreference;
        return View();
    }

    // Step 6: Check status manually
    [HttpGet]
    public async Task<IActionResult> CheckStatus(string keyreference, string orderId)
    {
        var payload = new
        {
            generateRefOrder = keyreference,
            ecommerceId = _config["CbPay:EcommerceId"],
            orderId = orderId
        };

        var result = await _service.CheckStatusAsync(payload);
        return Content(result, "application/json");
    }
}