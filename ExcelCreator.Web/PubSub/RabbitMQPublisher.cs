using ExcelCreator.Web.Services;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Shared;

namespace ExcelCreator.Web.PubSub
{
    public class RabbitMQPublisher
    {
        
            private readonly RabbitMQClientService _rabbitMQClientService;

            public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
            {
                _rabbitMQClientService = rabbitMQClientService;
            }

            public void Publish(CreateExcelMessage message)
            {
                IModel channel = _rabbitMQClientService.Connect();
                string mesaj = JsonSerializer.Serialize(message);
                byte[] byteMesaj = Encoding.UTF8.GetBytes(mesaj);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel
                    .BasicPublish
                    (
                        exchange: RabbitMQClientService.ExchangeName,
                        basicProperties: properties,
                        routingKey: RabbitMQClientService.RoutingKey, body: byteMesaj
                    );

            }
        }
    
}
