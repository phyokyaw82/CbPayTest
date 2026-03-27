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
            notifyUrl = $"{Request.Scheme}://{Request.Host}/Payment/Callback?orderId={req.OrderId}",
            signature = _config["CbPay:Signature"],
            subMerId = _config["CbPay:SubMerId"]
        };

        var cbpayResponse = await _service.RequestPaymentAsync(payload);
        if (!cbpayResponse.responseMessage.Equals("Operation Success.", StringComparison.OrdinalIgnoreCase))
        {
            return View("Error", new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                Message = cbpayResponse.responseMessage
            });
        }
        // STEP 3: Redirect to CBPay Deeplink (IMPORTANT PART)
        // var deeplink = $"cbuat://pay?keyreference={generateRefOrder}";
        var deeplink = "https://cbpay-deeplink-test.netlify.app/";
        return Redirect(deeplink);
    }

    // Step 4: CBPay backend calls this after success
    [HttpPost]
    public async Task<IActionResult> Callback(string orderId)
    {
        try
        {
            // 1. parse data
            // 2. verify signature (VERY important)
            // 3. update order in DB
            // 4. avoid duplicate processing
            return View("CbPaySuccess", orderId);
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