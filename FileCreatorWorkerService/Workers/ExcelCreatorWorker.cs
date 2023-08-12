using ClosedXML.Excel;
using FileCreatorWorkerService.Database;
using FileCreatorWorkerService.Entities;
using FileCreatorWorkerService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Data;
using System.Text;
using System.Text.Json;

namespace FileCreatorWorkerService.Workers
{
    public class ExcelCreatorWorker : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly IServiceProvider _serviceProvider;
        //private readonly ILogger _logger;
        private IModel _channel;

        public ExcelCreatorWorker(RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
            //_logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AsyncEventingBasicConsumer consumer = new (_channel);

            _channel.BasicConsume(queue: RabbitMQClientService.QueueName, autoAck: false, consumer: consumer);

            consumer.Received += Consumer_Received;
            
            return Task.CompletedTask;
        }

        // data table'i xml'e cevirebiliriz.
        private DataTable GetDataTable(string tableName) 
        {
            List<Urun> urunler;

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            urunler = dbContext.Uruns.ToList();

            // Geriye data table donecegiz
            // belirledigimiz sutunlari alacagiz
            DataTable table = new DataTable { TableName = tableName };
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("ad", typeof(string));
            table.Columns.Add("renk", typeof(string));
            table.Columns.Add("ucret", typeof(decimal));
            table.Columns.Add("tur", typeof(int));
            table.Columns.Add("is_aktif", typeof(bool));
            table.Columns.Add("op_time", typeof(DateTime));
            table.Columns.Add("is_deleted", typeof(bool));

            urunler.ForEach(urun =>
            {
                table.Rows.Add(urun.Id, urun.Ad, urun.Renk, urun.Ucret, urun.Tur, urun.IsAktif, urun.OpTime, urun.IsDeleted);
            });

            return table;

        }

        private async Task Consumer_Received(object s, BasicDeliverEventArgs e)
        {
            // excel create edilir
            await Task.Delay(5000);
            var data = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(e.Body.ToArray()));

            // olusturdugumuz excel'i memory stream'de tutacagiz
            using var ms = new MemoryStream();

            // xml work book olusturulur
            var wb = new XLWorkbook();
            // data table data set'e eklenir
            var ds = new DataSet();
            ds.Tables.Add(GetDataTable("urunler"));

            // tabloyu olusturur
            wb.Worksheets.Add(ds);

            // şuan excel memory stream'de yani bellekte
            wb.SaveAs(ms);

            // endpoint'e gonderecegizi iformfile nesnesi olusturulur
            // file byte array olarak gonderilir
            // file --> enpoint alacagı parametre adi
            MultipartFormDataContent multipartFormDataContent = new();
            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file", Guid.NewGuid().ToString()+".xlsx");

            var endpointUrl = "http://localhost:5001/api/files/upload";

            using var httipClient = new HttpClient();

            // post istegi yapilir
            // fileId query string olarak gonderilir
            var response = await httipClient.PostAsync(requestUri: $"{endpointUrl}?fileId={data.FileId}", content: multipartFormDataContent);

            // mesaj basarili islendiyse rabbitmq bilgilendirilir
            if (response.IsSuccessStatusCode)
            {
                //_logger.LogInformation($"File (Id: {data.FileId}) was created by successful");
                _channel.BasicAck(deliveryTag: e.DeliveryTag,multiple: false);
            }
        }
    }
}
