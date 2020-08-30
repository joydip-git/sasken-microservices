using EventBusRabbitMQ;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.API.RabbitMQ
{
    public class EventBusRabbitMQConsumer
    {
        private readonly IRabbitMQConnection _connection;

        public EventBusRabbitMQConsumer(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void Consume()
        {
            try
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(EventBusConstants.BASKET_CHECKOUT_QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;
                    channel.BasicConsume(queue: EventBusConstants.BASKET_CHECKOUT_QUEUE, autoAck: true, consumer: consumer, exclusive: false, noLocal: false, arguments: null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == EventBusConstants.BASKET_CHECKOUT_QUEUE)
            {
                var message = Encoding.UTF8.GetString(e.Body.Span);
                var basketCheckouEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);
               
            }
        }

        public void Disconnect() { }
    }
}
