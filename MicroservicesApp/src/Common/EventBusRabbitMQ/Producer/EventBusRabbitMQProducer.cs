using EventBusRabbitMQ.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace EventBusRabbitMQ.Producer
{
    public class EventBusRabbitMQProducer
    {
        private readonly IRabbitMQConnection _connection;
        public EventBusRabbitMQProducer(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
        public void PublishBasketCheckout(string queueName, BasketCheckoutEvent model)
        {
            using (IModel channel = _connection.CreateModel())
            {

                //code to publish your data (by converting into a message) to the queue using channel
                //declare a queue --> some information re required
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                
                //create a message
                var message = JsonConvert.SerializeObject(model);
                var body = Encoding.UTF8.GetBytes(message);

                //configuer the channel
                //contenttype
                //persistent
                //deliverymode
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;


                channel.ConfirmSelect();
                //publish
                channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties, body: body);
                channel.WaitForConfirmsOrDie();

                //register a callback for the channel which will act upon receiving an ack
                channel.BasicAcks += Channel_BasicAcks;
                //wait for confirmation
                channel.ConfirmSelect();
            }
        }

        private void Channel_BasicAcks(object sender, RabbitMQ.Client.Events.BasicAckEventArgs e)
        {
            Console.WriteLine("sent rabbitmq");
        }
    }
}
