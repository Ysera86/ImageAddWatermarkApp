using ImageAddWatermarkApp.Web.BackgroundServices;
using ImageAddWatermarkApp.Web.Models;
using ImageAddWatermarkApp.Web.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName: "productDb");
});


#region RabbitMQ

var rabbitMQConnection = builder.Configuration.GetSection("RabbitMQConnection").Get<RabbitMQConnection>();
builder.Services.AddSingleton(sp =>
{
    return new ConnectionFactory() { Port = rabbitMQConnection.Port, HostName = rabbitMQConnection.HostName, UserName = rabbitMQConnection.UserName, Password = rabbitMQConnection.Password };
});

builder.Services.AddSingleton<RabbitMQClientService>();


builder.Services.AddSingleton<RabbitMQPublisher>();


#endregion

#region Background Service

builder.Services.AddHostedService<ImageWatermarkProcessBackgroundService>();

#endregion

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
