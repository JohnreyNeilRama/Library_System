/**
 * Program.cs
 * 
 * This is the entry point of the ASP.NET Core web application.
 * It configures services, dependency injection, middleware, and the request pipeline.
 */

using LibrarySystem.Data;
using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with SQL Server
// The connection string is retrieved from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the database-backed library service for Dependency Injection
// This allows controllers to use ILibraryService without knowing the implementation
builder.Services.AddScoped<ILibraryService, DbLibraryService>();

// Add support for Controllers and Views (MVC)
builder.Services.AddControllersWithViews();

// Configure Authentication using Cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect here if user is not authenticated
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/Index";
        options.Cookie.Name = "LibrarySystemAuth";
        options.Cookie.HttpOnly = true; // Security: cookie not accessible via JS
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true; // Renew cookie if user is active
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files from wwwroot
app.UseRouting();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map static assets for performance (available in newer .NET versions)
app.MapStaticAssets();

// Define the default routing pattern
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
