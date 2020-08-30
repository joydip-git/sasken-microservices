using EventBusRabbitMQ;
using EventBusRabbitMQ.Common;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using EventBusRabbitMQ.Events;
using System.Runtime.CompilerServices;
using AutoMapper;
using Ordering.Application.Commands;
using MediatR;
using Ordering.Core.Repositories;

namespace Ordering.API.RabbitMQ
{
    public class EventBusRabbitMQConsumer
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IOrderRepository _repository;

        public EventBusRabbitMQConsumer(IRabbitMQConnection connection, IMapper mapper, IMediator mediator, IOrderRepository repository)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator?? throw new ArgumentNullException(nameof(mediator));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Consume()
        {
            try
            {
                var channel = _connection.CreateModel();
                channel.QueueDeclare(EventBusConstants.BASKET_CHECKOUT_QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(queue: EventBusConstants.BASKET_CHECKOUT_QUEUE, autoAck: true, consumer: consumer, exclusive: false, noLocal: false, arguments: null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == EventBusConstants.BASKET_CHECKOUT_QUEUE)
            {
                var message = Encoding.UTF8.GetString(e.Body.Span);
                var basketCheckOutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);
                var command = _mapper.Map<CheckoutOrderCommand>(basketCheckOutEvent);
                var result = await _mediator.Send(command);
            }
        }

        public void Disconnect()
        {
            _connection.Dispose();
        }
    }
}
