using RabbitMQ.Client;
using System;

namespace EventBusRabbitMQ
{
    public interface IRabbitMQConnection : IDisposable
    {
        bool TryConnect();
        IModel CreateModel();
        bool IsConnected { get; }
    }
}
