
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace StockPriceApi.Listener
{
    public class StockPriceListener
    {
        private readonly string _exchangeName;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private EventingBasicConsumer _consumer;

        public StockPriceListener(IConfiguration configuration)
        {
            var configSection = configuration.GetSection("RabbitMQ");
            var connectSection = configuration.GetSection("ExchangeConnection");

            var factory = new ConnectionFactory
            {
                HostName = connectSection.GetValue<string>("hostname"),
                Port = connectSection.GetValue<int>("port"),
                UserName = configSection.GetValue<string>("Username"),
                Password = configSection.GetValue<string>("Password")
            };

            Console.WriteLine($"Target Host: {configuration.GetValue<string>("RabbitMQHost")}");
            Console.WriteLine($"Target Port: {configuration.GetValue<string>("RabbitMQPort")}");

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = configSection.GetValue<string>("ExchangeName");
        }

        public void Start()
        {
            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Fanout
            );

            var queueName = _channel.QueueDeclare().QueueName;
            Console.WriteLine($"Queue: {queueName}");
            _channel.QueueBind(
                queue: queueName,
                exchange: _exchangeName,
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

            _connection.Close();
            _connection.Dispose();
        }

        void HandleMessageReceive(object sender, BasicDeliverEventArgs e)
        {
            var bodyContents = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine($"Message Received: {bodyContents}");
        }
    }
}