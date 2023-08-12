using ExcelCreator.Web.Database;
using ExcelCreator.Web.Hubs;
using ExcelCreator.Web.PubSub;
using ExcelCreator.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
});

// identity ile user ve role verilir
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; // email zorunlu
}).AddEntityFrameworkStores<AppIdentityDbContext>();  // identity bilgileri ef core da kayit edilecek

builder.Services.AddSingleton(sp =>
{
    return new ConnectionFactory()
    {
        Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQUri")),
        DispatchConsumersAsync = true // async metot kullanildigi bildirilir
    };
});

builder.Services.AddSignalR();

builder.Services.AddSingleton<RabbitMQClientService>();

builder.Services.AddSingleton<RabbitMQPublisher>();


var app = builder.Build();
// app ayaða kalkarken migrate yapilsin
using var scope = app.Services.CreateScope();
{
    var appDbcontext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    // identity'nin sunmus oldugu userManager class i var
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // migrate edilir
    appDbcontext.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<FileHub>("/filehub");

app.Run();
