using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Threading;

namespace EventBusRabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _isDisposed;

        public RabbitMQConnection(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            if (!IsConnected)
            {
                TryConnect();
            }
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_isDisposed;
            }
        }

        public IModel CreateModel()
        {
            try
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("No rabbitmq connection available to create channel");
                }
                return _connection.CreateModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                Thread.Sleep(2000);
                _connection = _connectionFactory.CreateConnection();
            }
            if (IsConnected)
            {
                Console.WriteLine($"RabbitMq connection got a connection {_connection.Endpoint.HostName} and is subscribed ");
                return true;
            }
            else
            {
                Console.WriteLine($"RabbitMq connection couldn't be established ");
                return false;
            }
        }
    }
}
