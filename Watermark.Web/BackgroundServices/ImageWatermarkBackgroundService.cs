using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Watermark.Web.Contract;
using Watermark.Web.Services;

namespace Watermark.Web.BackgroundServices
{
    // Background service program.cs 'de bildirilir
    public class ImageWatermarkBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<ImageWatermarkBackgroundService> _logger;
        private IModel _channel;

        public ImageWatermarkBackgroundService(RabbitMQClientService rabbitMQClientService, ILogger<ImageWatermarkBackgroundService> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // consumer olusturulur
            AsyncEventingBasicConsumer consumer = new(_channel);

            // consumer hangi kuyrugu tuketicek ayarlanir
            _channel.BasicConsume(queue: RabbitMQClientService.QueueName, consumer: consumer, autoAck: false);

            // tuketilir
            consumer.Received += Consumer_Receive;

            return Task.CompletedTask;

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // startup
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            return base.StartAsync(cancellationToken);
        }

        private Task Consumer_Receive(object sender, BasicDeliverEventArgs e)
        {

            try
            {
                // resime yazi ekleme işlemi yapilir
                ImageCreatedEvent imageCreatedEvent = JsonSerializer.Deserialize<ImageCreatedEvent>(Encoding.UTF8.GetString(e.Body.Span));
                // image'ın path'i alinir
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageCreatedEvent.ImageName);

                // image alinir
                using var image = Image.FromFile(path);

                // image'a yazi yazmak icin grafik oluturulur
                using var grafik = Graphics.FromImage(image);

                string yazilacakYazi = "Mustfa SEYMEN";

                // font
                var font = new Font(family: FontFamily.GenericMonospace, emSize: 40, style: FontStyle.Bold, unit: GraphicsUnit.Pixel);

                // yazinin boyutu
                var textSize = grafik.MeasureString(yazilacakYazi, font);

                // renk
                var color = Color.FromArgb(128, 139, 58, 58);

                // fırca
                var brush = new SolidBrush(color);

                // yazinin pozisyonu
                // sağ alt tarafına 30 - 30 px boşluk birakilir
                // yazinin boyutu 50 px
                var position = new Point(image.Width - ((int)textSize.Width + 30), image.Height - ((int)textSize.Height + 30));

                // yazi cizilir
                grafik.DrawString(s: yazilacakYazi, font: font, brush: brush, point: position);

                // image kayit edilir
                image.Save("wwwroot/watermark/" + imageCreatedEvent.ImageName);

                //basarili bir sekilde islenirse rabbimq ya bidirim gonderilir

                _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);

                _logger.LogInformation("image'a watermark eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.CompletedTask;



        }
    }
}
