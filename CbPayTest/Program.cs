using CbPayTest.Services;

var builder = WebApplication.CreateBuilder(args);

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