using RabbitMQ.Client;

namespace ExcelCreator.Web.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ILogger<RabbitMQClientService> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "excel_exchange";
        public static string RoutingKey = "excel_route";
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


            // create exchange
            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);

            // create queue
            _channel.QueueDeclare(queue: QueueName, durable: true, autoDelete: false, exclusive: false);

            // create binding
            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);

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
