
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockPriceApi.Hubs;
using StockPriceApi.Models;

namespace StockPriceApi.Listener
{
    public class StockPriceListener
    {
        private readonly IHubContext<StockPriceHub> _stockPriceHub;
        private readonly string _exchangeName;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private EventingBasicConsumer _consumer;

        public StockPriceListener(IConfiguration configuration, IHubContext<StockPriceHub> hubContext)
        {
            var configSection = configuration.GetSection("RabbitMQ");
            var connectSection = configuration.GetSection("ExchangeConnection");
            var hostname = connectSection.GetValue<string>("RabbitMQHost");
            var port = connectSection.GetValue<int>("RabbitMQPort");
            var username = configSection.GetValue<string>("Username");
            var password = configSection.GetValue<string>("Password");

            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hostname: {hostname}");
            Console.WriteLine($"Port: {port}");

            var factory = new ConnectionFactory
            {
                HostName = hostname,
                Port = port,
                UserName = username,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = configSection.GetValue<string>("ExchangeName");

            _stockPriceHub = hubContext;
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
            var stockPrice = JsonConvert.DeserializeObject<StockPriceChange>(bodyContents);

            _stockPriceHub.Clients.All.SendAsync(nameof(IPriceChangeClient.ReceiveStockPrice), stockPrice)
                .GetAwaiter()
                .GetResult();
        }
    }
}