using CbPayTest.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<CbPayService>();

var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Add($"http://0.0.0.0:{port}");

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Payment}/{action=Index}/{id?}");

app.Run();