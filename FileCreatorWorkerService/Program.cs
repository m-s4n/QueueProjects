using FileCreatorWorkerService.Database;
using FileCreatorWorkerService.Services;
using FileCreatorWorkerService.Workers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services.AddHostedService<ExcelCreatorWorker>();
        services.AddSingleton(sp =>
        {
            
            return new ConnectionFactory()
            {
                Uri = new Uri(configuration.GetConnectionString("RabbitMQUri")),
                DispatchConsumersAsync = true // async metot kullanildigi bildirilir
            };
        });

        services.AddSingleton<RabbitMQClientService>();
        services.AddDbContext<AppDbContext>(options =>
        {

            options.UseNpgsql(configuration.GetConnectionString("AppDb"));
        });


    })
    .Build();

host.Run();
