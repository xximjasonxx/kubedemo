
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace StockPriceApi.Listener
{
    public class StockPriceListener : IHostedService
    {
        private readonly string ExchangeName = "TheExchange";

        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = BuildConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Direct
            );

            var queueName = _channel.QueueDeclare().QueueName;
            Console.WriteLine($"Queue: {queueName}");

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += HandleMessageReceive;
            _consumer.Registered += (obj, ev) => Console.WriteLine("Registered");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_consumer != null)
            {
                _consumer.Received -= HandleMessageReceive;
            }

            _channel?.Dispose();
            _connection?.Dispose();

            return Task.CompletedTask;
        }

        void HandleMessageReceive(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine("Message Received");
        }

        IConnection BuildConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "192.168.99.100",
                Port = 30046,
                UserName = "rabbit",
                Password = "rabbit"
            };

            return factory.CreateConnection();
        }
    }
}