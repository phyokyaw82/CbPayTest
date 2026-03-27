using CbPayTest.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//// 🔥 Configure Serilog BEFORE building app
//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Information()
//    .WriteTo.Console()
//    .WriteTo.File(
//        path: "Logs/cbpay-log-.txt",
//        rollingInterval: RollingInterval.Day,
//        retainedFileCountLimit: 7)
//    .CreateLogger();

//builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<CbPayService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Payment}/{action=Index}/{id?}");

// ✅ Render provides PORT, local does not
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

// IMPORTANT: bind AFTER build but BEFORE Run
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");
//http://localhost:10000
app.Run();