using CbPayTest.Models;
using CbPayTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace CbPayTest.Controllers;

public class PaymentController : Controller
{
    private readonly CbPayService _service;
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        CbPayService service,
        IConfiguration config,
        ILogger<PaymentController> logger)
    {
        _service = service;
        _config = config;
        _logger = logger;
    }

    // Step 1: Show form
    public IActionResult Index()
    {
        _logger.LogInformation("Payment Index page loaded");
        return View();
    }

    // Step 2: Create Payment Order
    [HttpPost]
    public async Task<IActionResult> CreatePayment(PaymentRequest req)
    {
        _logger.LogInformation("Creating payment for OrderId: {OrderId}", req.OrderId);

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

        var response = await _service.RequestPaymentAsync(payload);

        _logger.LogInformation("CBPay Response Code: {Code}", response.responseCode);
        _logger.LogInformation("CBPay Response Message: {Msg}", response.responseMessage);
        _logger.LogInformation("CBPay GenerateRefOrder: {Ref}", response.GenerateRefOrder);
        _logger.LogInformation("CBPay NotifyUrl: {Url}", payload.notifyUrl);

        var deeplink = "https://cbpay-deeplink-test.netlify.app/";
        return Redirect(deeplink);
    }

    // Step 4: CBPay backend callback
    [HttpPost]
    public async Task<IActionResult> Callback()
    {
        var body = await new StreamReader(Request.Body).ReadToEndAsync();

        _logger.LogInformation("CBPay Callback received: {Body}", body);

        return Ok();
    }

    // Step 5: CBPay redirects user back here
    [HttpGet]
    public IActionResult CbPaySuccess(string keyreference)
    {
        _logger.LogInformation("User redirected from CBPay. KeyReference: {Key}", keyreference);

        ViewBag.KeyReference = keyreference;
        return View();
    }

    // Step 6: Check status manually
    [HttpGet]
    public async Task<IActionResult> CheckStatus(string keyreference, string orderId)
    {
        _logger.LogInformation("Checking payment status. KeyReference: {Key}, OrderId: {OrderId}",
            keyreference, orderId);

        var payload = new
        {
            generateRefOrder = keyreference,
            ecommerceId = _config["CbPay:EcommerceId"],
            orderId = orderId
        };

        var result = await _service.CheckStatusAsync(payload);

        _logger.LogInformation("Status API response received");

        return Content(result, "application/json");
    }
}