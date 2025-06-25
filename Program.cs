using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Helpers;
using ClientPortalBifurkacioni.Models.CustomModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<PaymentSettings>(builder.Configuration.GetSection("PaymentSettings"));
builder.Services.AddScoped<PaymentHelper>();
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings")
);
builder.Services.AddControllersWithViews();

ConfigurationAccessor.SetConfiguration(builder.Configuration);

var app = builder.Build();

// Use error handling and HTTPS redirection
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Authenticate}/{id?}");

app.Run();