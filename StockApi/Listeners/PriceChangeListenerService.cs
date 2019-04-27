
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace StockApi.Listeners
{
    public class PriceChangeListenerService : IHostedService
    {
        private IConnection _exchangeConnection;
        private IModel _exchangeChannel;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Hosted Service starting");
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.99.100",
                Port = 30046,
                UserName = "rabbit",
                Password = "rabbit"
            };

            _exchangeConnection = factory.CreateConnection();
            _exchangeChannel = _exchangeConnection.CreateModel();

            _exchangeChannel.ExchangeDeclare("TheExchange",
                type: ExchangeType.Direct,
                durable: false,
                autoDelete: false);

            _exchangeChannel.QueueBind(_exchangeChannel.QueueDeclare().QueueName,
                exchange: "TheExchange",
                routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(_exchangeChannel);
            consumer.Received += HandleEventReceived;

            _exchangeChannel.BasicConsume(_exchangeChannel.QueueDeclare().QueueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        private void HandleEventReceived(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine("Message Received");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Hosted Service stopping");
            return Task.CompletedTask;
        }
    }
}