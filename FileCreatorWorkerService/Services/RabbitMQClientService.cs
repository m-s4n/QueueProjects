using RabbitMQ.Client;

namespace FileCreatorWorkerService.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ILogger<RabbitMQClientService> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        public static string QueueName = "excel_queue";

        // exchange-queueu olusturulması
        // ve binding islemi burada yapilacak

        public RabbitMQClientService(ConnectionFactory factory, ILogger<RabbitMQClientService> logger)
        {
            _factory = factory;
            _logger = logger;
        }


        public IModel Connect()
        {
            _connection = _factory.CreateConnection();
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            // create channel
            _channel = _connection.CreateModel();
            // queue, exchange tanimlama ve binding islemi producer tarafinda yapildi

            _logger.LogInformation("RabbitMQ ile baglanti kuruldu");

            return _channel;

        }

        public void Dispose()
        {
            // dispose olduğunda baglantilar kapatilir
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile baglanti kapandi");
        }
    }
}
