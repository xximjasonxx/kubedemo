
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace StockPriceApi.Listener
{
    public class StockPriceListener
    {
        private readonly string ExchangeName = "TheExchange";

        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        public void Start()
        {
            _connection = BuildConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Fanout
            );

            var queueName = _channel.QueueDeclare().QueueName;
            Console.WriteLine($"Queue: {queueName}");
            _channel.QueueBind(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: string.Empty
            );

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += HandleMessageReceive;
            _consumer.Registered += (obj, ev) => Console.WriteLine("Registered");

            _channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: _consumer
            );
        }

        public void Stop()
        {
            _consumer.Received -= HandleMessageReceive;
            _consumer = null;

            _channel.Close();
            _channel.Dispose();
            _channel = null;

            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        void HandleMessageReceive(object sender, BasicDeliverEventArgs e)
        {
            var bodyContents = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine($"Message Received: {bodyContents}");
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