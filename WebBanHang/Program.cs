﻿using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Repository;
using WebBanHang.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add database config
var connectionString = builder.Configuration.GetConnectionString("SnackStoreContext");
builder.Services.AddDbContext<SnackStoreContext>(x => x.UseSqlServer(connectionString));

//Add repository config
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

//Add session config
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.IsEssential = true;
});

//Connect VNPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();


var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Admin" },
    constraints: new { area = "Admin" });

app.Run();
