using Microsoft.EntityFrameworkCore;
using TicketOffice.Data;
using TicketOffice.Models;
using TicketOffice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddSessionStateTempDataProvider();

builder.Services.AddDbContext<TicketOfficeContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TicketOfficeContext")));

builder.Services.AddScoped<UserValidationService>();
builder.Services.AddScoped<PdfService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AutoBus.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
SeedData.Initialize(services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapRazorPages();

app.Run();

