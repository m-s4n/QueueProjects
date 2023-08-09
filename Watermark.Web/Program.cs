using Watermark.Web.Database;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Watermark.Web.Services;
using Watermark.Web.PubSub;
using Watermark.Web.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// background servis


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName: builder.Configuration.GetConnectionString("InMemoryDbName"));
});

builder.Services.AddSingleton(sp =>
{
    return new ConnectionFactory()
    {
        Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQUri")),
        DispatchConsumersAsync = true // async metot kullanildigi bildirilir
    };
});

builder.Services.AddSingleton<RabbitMQClientService>();

builder.Services.AddSingleton<RabbitMQPublisher>();

builder.Services.AddHostedService<ImageWatermarkBackgroundService>();

var app = builder.Build();

// uygulama ayaga kalkarken db olusturulur
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
