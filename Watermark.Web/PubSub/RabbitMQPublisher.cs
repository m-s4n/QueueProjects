using Watermark.Web.Services;
using Watermark.Web.Contract;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace Watermark.Web.PubSub
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(ImageCreatedEvent imageCreatedEvent)
        {
            IModel channel = _rabbitMQClientService.Connect();
            string mesaj = JsonSerializer.Serialize(imageCreatedEvent);
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
